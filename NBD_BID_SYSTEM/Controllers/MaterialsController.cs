using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;

namespace NBD_BID_SYSTEM.Controllers
{
    public class MaterialsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public MaterialsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            var nBDBidSystemContext = _context.Materials.Include(m => m.Bid).Include(m => m.Inventory);
            return View(await nBDBidSystemContext.ToListAsync());
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.Bid)
                .Include(m => m.Inventory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Materials/Create
        public IActionResult Create(int? BidID)
        {
            Material a = new Material()
            {
                BidID = BidID.GetValueOrDefault()
            };
            ViewData["BidID"] = new SelectList(_context.Bids, "ID", "ID");
            ViewData["InventoryID"] = new SelectList(_context.Inventories, "ID", "Code");
            return View(a);
        }

        // POST: Materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Price,Quantity,BidID,InventoryID")] Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Bids", new { ID = material.BidID });
            }
            ViewData["BidID"] = new SelectList(_context.Bids, "ID", "ID", material.BidID);
            ViewData["InventoryID"] = new SelectList(_context.Inventories, "ID", "Code", material.InventoryID);
            return View(material);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            ViewData["BidID"] = new SelectList(_context.Bids, "ID", "ID", material.BidID);
            ViewData["InventoryID"] = new SelectList(_context.Inventories, "ID", "Code", material.InventoryID);
            return View(material);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Price,Quantity,BidID,InventoryID")] Material material)
        {
            if (id != material.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Bids", new { ID = material.BidID });
            }
            ViewData["BidID"] = new SelectList(_context.Bids, "ID", "ID", material.BidID);
            ViewData["InventoryID"] = new SelectList(_context.Inventories, "ID", "Code", material.InventoryID);
            return View(material);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.Bid)
                .Include(m => m.Inventory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Details", "Bids", new { ID = material.BidID });
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.ID == id);
        }
    }
}
