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
    /// <summary>

    /// </summary>
    public class BidLaborsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public BidLaborsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: BidLabors
        public async Task<IActionResult> Index()
        {
            
            var bidLabors= _context.BidLabors.Include(b => b.Bid)
                .ThenInclude(b => b.Project)
                .Include(b => b.Labor);
            return View(await bidLabors.ToListAsync());
        }

        // GET: BidLabors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bidLabor = await _context.BidLabors
                .Include(b => b.Bid)
                .ThenInclude(b => b.Project)
                .Include(b => b.Labor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bidLabor == null)
            {
                return NotFound();
            }

            return View(bidLabor);
        }

        // GET: BidLabors/Create
        // public IActionResult Create()
        //{

          //  PopulateDropDown();
          //  return View();
        //}

        // POST: BidLabors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,HoursWorked,ExtPrice,LaborID,BidID")] BidLabor bidLabor)
        //{
          //  try
           // {
            //    if (ModelState.IsValid)
             //   {
              //      _context.Add(bidLabor);
               //     await _context.SaveChangesAsync();
                 //   return RedirectToAction(nameof(Index));
                //}
            //}
            //catch (DbUpdateException)
            //{
              //  ModelState.AddModelError("", "Unable to save the changes. Please try again later");
            //}
            //PopulateDropDown(bidLabor);
            //return View(bidLabor);
        //}

        // GET: BidLabors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bidLabor = await _context.BidLabors
                .Include(b=>b.Labor)
                .Include(b => b.Bid)
                .FirstOrDefaultAsync(b => b.ID == id);
            if (bidLabor == null)
            {
                return NotFound();
            }
            ViewData["BidID"] = bidLabor.BidID;
            ViewData["LaborID"] = bidLabor.LaborID;
            return View(bidLabor);
        }

        // POST: BidLabors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var bidLaborToUpdate = await _context.BidLabors
                .Include(b=> b.Labor)
                .Include(b =>b.Bid)
                .FirstOrDefaultAsync(b => b.ID == id);
            if (bidLaborToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<BidLabor>(bidLaborToUpdate, "", b => b.BidID, b => b.LaborID, b => b.HoursWorked)) 
            {
                try
                {
                    //calculating ExtPrice
                    bidLaborToUpdate.ExtPrice = bidLaborToUpdate.HoursWorked * _context.Labors.FirstOrDefault(l => l.ID == bidLaborToUpdate.LaborID).Price;

                    _context.Update(bidLaborToUpdate);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BidLaborExists(bidLaborToUpdate.ID))
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
                    ModelState.AddModelError("", "Unable to save the changes. Please try again later");
                }

            }
            ViewData["BidID"] = bidLaborToUpdate.BidID;
            ViewData["LaborID"] = bidLaborToUpdate.LaborID;
            return View(bidLaborToUpdate);
        }

        // GET: BidLabors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bidLabor = await _context.BidLabors
                .Include(b => b.Bid)
                .ThenInclude(b => b.Project)
                .Include(b => b.Labor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bidLabor == null)
            {
                return NotFound();
            }

            return View(bidLabor);
        }

        // POST: BidLabors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bidLabor = await _context.BidLabors.FindAsync(id);
            try
            {
                _context.BidLabors.Remove(bidLabor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again later!");
            }
            return View(bidLabor);
        }

        private bool BidLaborExists(int id)
        {
            return _context.BidLabors.Any(e => e.ID == id);
        }

        private SelectList LaborSelectList(BidLabor bidlabor)
        {
            var LabourList = new SelectList(_context.Labors, "ID", "Type", bidlabor?.LaborID);
            return LabourList;
        }
        private SelectList BidSelectList(BidLabor bidLabor)
        {
            var bidList = new SelectList(_context.Bids, "ID", "ID", bidLabor?.BidID);
            return bidList;
        }

        private void PopulateDropDown(BidLabor bidLabor = null)
        {
            ViewData["BidID"] = BidSelectList(bidLabor);
            ViewData["LaborID"] = LaborSelectList(bidLabor);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }
    }
}
