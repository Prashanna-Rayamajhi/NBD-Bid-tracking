using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Controllers
{
    public class StaffsAccountController : Controller
    {
        private readonly  NBDBidSystemContext _context;
        private readonly ApplicationDbContext _identityContext;

        public StaffsAccountController(NBDBidSystemContext context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }
        public async Task<IActionResult> Index()
        {
           return View(await _context.Staffs.Include(p=>p.Position).ToListAsync());
        }

        public IActionResult Create(string Email)
        {
            Staff staff = new Staff() {
                Email = Email
            };
            PopulatePositionList(staff);
            return View(staff);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Email,PositionID")] Staff staff)
        {
           
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(staff);
                    await _context.SaveChangesAsync();
                    return Redirect("~/Identity/Account/Register?msg="+staff.Email);
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            PopulatePositionList(staff);
            return View(staff);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(p=>p.Position).FirstOrDefaultAsync(p=>p.ID == id);
            if (staff == null)
            {
                return NotFound();
            }
            PopulatePositionList();
            return View(staff);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active)
        {
            var employeeToUpdate = await _context.Staffs
                .Include(p=>p.Position)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            //Check to see if you are making them inactive
            if (employeeToUpdate.Active == true && Active == false)
            {
                //This deletes the user's login from the security system
                await DeleteIdentityUser(employeeToUpdate.Email);

            }

            if (await TryUpdateModelAsync<Staff>(employeeToUpdate, "",
                e => e.FirstName, e => e.LastName, e => e.Phone, e => e.PositionID, e => e.Active))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            PopulatePositionList(employeeToUpdate);
            return View(employeeToUpdate);

        }
        private async Task DeleteIdentityUser(string Email)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.Email == Email).FirstOrDefaultAsync();
            if (userToDelete != null)
            {
                _identityContext.Users.Remove(userToDelete);
                await _identityContext.SaveChangesAsync();
            }
        }
        private bool EmployeeExists(int id)
        {
            return _context.Staffs.Any(e => e.ID == id);
        }
        private SelectList PositionSelectList(Staff staff)
        {
            return new SelectList(_context.Positions.OrderBy(s => s.Description), "ID", "Description", staff?.PositionID);
        }
        private void PopulatePositionList(Staff staff = null)
        {
            ViewData["PositionID"] = PositionSelectList(staff);
        }
    }
}
