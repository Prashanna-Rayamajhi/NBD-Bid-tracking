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
using Microsoft.AspNetCore.Authorization;


namespace NBD_BID_SYSTEM.Controllers
{
    public class InventoryController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public InventoryController(NBDBidSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Details));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.InventoryType)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);
            if (inventory == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(inventory);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Code,Name,Description,Size,Price,InventoryTypeID")] Inventory inventory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(inventory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(inventory);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            var inventory = await _context.Inventories
                .Include(i => i.InventoryType)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id)
        {
            var inventoryToUpdate = await _context.Inventories.FindAsync(id);

            if (inventoryToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Inventory>(inventoryToUpdate, "",
                i => i.Code, i => i.Name, i => i.Description, i => i.Size, i => i.Price))
            {
                try
                {
                    _context.Update(inventoryToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventoryToUpdate.ID))
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
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(inventoryToUpdate);
        }

        private bool InventoryExists(int iD)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Contingents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            try
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Contingent. Remember, you cannot delete the Contingent of any Athlete in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(inventory);

        }

    }





}