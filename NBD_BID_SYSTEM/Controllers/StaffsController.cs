using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;
using NBD_BID_SYSTEM.Utilities;

namespace NBD_BID_SYSTEM.Controllers
{
    //this controller is for account management of staff
    public class StaffsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public StaffsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: Staffs
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Details));
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details()
        {
            var staff = await _context.Staffs
           .Where(c => c.Email == User.Identity.Name)
           .Include(c => c.Position)
           .FirstOrDefaultAsync();
            if (staff == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            PopulatePositionList();
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Email,Active,PositionID")] Staff staff)
        {
            staff.Email = User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(staff);
                    await _context.SaveChangesAsync();
                    UpdateUserNameCookie(staff.FullName);
                    return RedirectToAction(nameof(Details));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit()
        {
            var staff = await _context.Staffs
                .Where(s=> s.Email == User.Identity.Name)
                .Include(s => s.Position)
                .FirstOrDefaultAsync();
            if (staff == null)
            {
                return RedirectToAction(nameof(Create));
            }
            PopulatePositionList();
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var staffToUpdate = await _context.Staffs
                .Include(s => s.Position)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<Staff>(staffToUpdate, "", s=>s.FirstName, s=>s.LastName, s=>s.Phone, s=> s.PositionID))
            {
                try
                {
                    _context.Update(staffToUpdate);
                    await _context.SaveChangesAsync();
                    UpdateUserNameCookie(staffToUpdate.FullName);
                    return RedirectToAction(nameof(Details));


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staffToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Something went wrong in the database.");
                }

            }
            PopulatePositionList();
            return View(staffToUpdate);
        }

        private SelectList PositionSelectList(Staff staff)
        {
            return new SelectList(_context.Positions.OrderBy(s => s.Description), "ID", "Description", staff?.PositionID);
        }

        private void PopulatePositionList(Staff staff = null)
        {
            ViewData["PositionID"] = PositionSelectList(staff);
        }
        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(e => e.ID == id);
        }

        private void UpdateUserNameCookie(string userName)
        {
            CookieHelper.SetCookieOptions(HttpContext, "userName", userName, 960);
        }

    }
}
