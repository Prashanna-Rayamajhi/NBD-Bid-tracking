using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;
using NBD_BID_SYSTEM.Utilities;
using NBD_BID_SYSTEM.ViewModels;

namespace NBD_BID_SYSTEM.Controllers
{
    //[Authorize]
    public class BidsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public BidsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: Bids
        public async Task<IActionResult> Index(string project, string amtGreaterThan, string btnAction,
            int? page, int? pageSizeID, string sortDirection = "asc", string sortField = "Name")
        {
            //clearing the cookie
            CookieHelper.SetCookieOptions(HttpContext, ControllerName() + "URL", "", -1);

            ViewData["IsFiltering"] = "";
            var bids = from b in _context.Bids.Include(b => b.Project)
                       .Include(b => b.BidLabors)
                       .ThenInclude(b => b.Labor)
                       .Include(b => b.ApproveBid)
                       .Include(b => b.BidStaffs)
                .ThenInclude(b => b.Staff)
                       select b;

            if (!String.IsNullOrEmpty(project))
            {
                bids = bids.Where(b => b.Project.Site.ToLower().Contains(project.ToLower()));
                ViewData["IsFiltering"] = "show";
            }
            if (!String.IsNullOrEmpty(amtGreaterThan))
            {
                bids = bids.Where(b => b.Amount >= Convert.ToDouble(amtGreaterThan));
                ViewData["IsFiltering"] = "show";
            }
            string[] possibleSortFileds = new[] { "Bid Amount", "Bid Date", "Project Site" };
            if (possibleSortFileds.Contains(btnAction)) //submit is done by sortFields
            {
                page = 1; //resetting the page to 1 when sortin is applied
                if (sortField == btnAction)
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                sortField = btnAction;

            }
            switch (sortField)
            {
                case "Bid Amount":
                    if (sortDirection == "asc")
                    {
                        bids = bids.OrderBy(b => b.Amount);
                    }
                    else
                    {
                        bids = bids.OrderByDescending(b => b.Amount);
                    }
                    break;
                case "Project Site":
                    bids = sortDirection == "asc" ? bids.OrderBy(b => b.Project.Site) : bids.OrderByDescending(b => b.Project.Site);
                    break;
                default:
                    bids = sortDirection == "asc" ? bids.OrderBy(b => b.Date.Year).ThenBy(b => b.Date.Month).ThenBy(b => b.Date.Day) : bids.OrderByDescending(b => b.Date.Year).ThenByDescending(b => b.Date.Month).ThenByDescending(b => b.Date.Day);
                    break;
            }

            ViewData["sortDirection"] = sortDirection;
            ViewData["sortField"] = sortField;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Bid>.CreateAsync(bids.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Bids/Details/5
        // [Authorize(Roles = "Admin,Designer,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids
                .Include(b => b.Project)
                .Include(b => b.BidLabors)
                .ThenInclude(b => b.Labor)
                .Include(b => b.ApproveBid)
                .Include(b => b.Materials).ThenInclude(b => b.Inventory)
                .Include(b => b.BidStaffs)
                .ThenInclude(b => b.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // GET: Bids/Create
        //[Authorize(Roles = "Admin,Designer")]
        public IActionResult Create(int? ProjectID)
        {
            Bid bid = new Bid
            {
                Date = DateTime.Today
            };
            if (ProjectID.HasValue)
            {
                bid.ProjectID = (int)ProjectID;
                PopulateDropDownList(bid);
            }
            else
            {
                PopulateDropDownList();
            }
            PopulateAssignedBidLabors(bid);
            PopulateAssignedBidStaffs(bid);

            ViewData["InventoryName"] = from s in _context.Inventories
                                        select s;
            ViewData["Quantity"] = _context.Materials.FirstOrDefault().Quantity;
            ViewData["ApproveBidID"] = _context.ApproveBids.FirstOrDefault(b => b.Status == "Pending").ID;

            return View(bid);
        }

        // POST: Bids/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Designer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Date,Amount,ProjectID,ApproveBidID")] Bid bid, string[] selectedOptions, string[] selectedBidstaff, string[] hoursWorked, string[] InventoryID, string[] Quantity, string[] Price)
        {
            try
            {

                //Add the selected Options
                if (selectedOptions != null && hoursWorked != null)
                {
                    for (int i = 0; i < selectedOptions.Length; i++)
                    {
                        var laborToAdd = new BidLabor { BidID = bid.ID, LaborID = int.Parse(selectedOptions[i]), HoursWorked = int.Parse(hoursWorked[i]), ExtPrice = _context.Labors.FirstOrDefault(c => c.ID == int.Parse(selectedOptions[i])).Price * int.Parse(hoursWorked[i]) }; //ext price and hours worked will be changed in bidlabor form
                        bid.BidLabors.Add(laborToAdd);
                    }
                }
                if (selectedBidstaff != null)
                {
                    foreach (var staff in selectedBidstaff)
                    {
                        var staffToAdd = new BidStaff { BidID = bid.ID, StaffID = int.Parse(staff) }; //ext price and hours worked will be changed in bidlabor form
                        bid.BidStaffs.Add(staffToAdd);
                    }
                }
                //for bidmaterial
                if(!(InventoryID == null && Quantity == null && Price == null))
                {
                    for(int i = 0; i < InventoryID.Length; i++)
                    {
                        var materialToAdd = new Material { BidID = bid.ID, InventoryID = int.Parse(InventoryID[i]), Quantity = int.Parse(Quantity[i]), Price = double.Parse(Price[i]) };
                        bid.Materials.Add(materialToAdd);
                    }
                  
                }
                if (ModelState.IsValid)
                {
                    _context.Add(bid);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { bid.ID });
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed:"))
                {
                    ModelState.AddModelError("", "Unable to save changes due to unique key violations");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateAssignedBidLabors(bid);
            PopulateAssignedBidStaffs(bid);
            PopulateDropDownList(bid);
            return View(bid);
        }

        // GET: Bids/Edit/5
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids
                .Include(b => b.Project)
                .Include(b => b.BidLabors)
                .ThenInclude(b => b.Labor)
                .Include(b => b.ApproveBid)
                .Include(b => b.BidStaffs)
                .ThenInclude(b => b.Staff)
                .Include(b => b.Materials)
                .ThenInclude(b => b.Inventory)
                .ThenInclude(b => b.InventoryType)
                .FirstOrDefaultAsync(b => b.ID == id);
            if (bid == null)
            {
                return NotFound();
            }
            PopulateAssignedBidLabors(bid);
            PopulateAssignedBidStaffs(bid);
            PopulateDropDownList(bid);
            if (!User.IsInRole("Admin"))
            {
                ViewData["ApproveBidID"] = _context.ApproveBids.FirstOrDefault(b => b.Status == "Pending").ID;
            }
            else
            {
                ViewData["ApproveBidID"] = new SelectList(_context.ApproveBids, "ID", "Status");
            }
            return View(bid);
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, string[] selectedBidstaff, string[] hoursWorked, string[] InventoryID, string[] Quantity, string[] Price)
        {

            var bidToUpdate = await _context.Bids
                .Include(b => b.BidLabors)
                .ThenInclude(b => b.Labor)
                .Include(b => b.ApproveBid)
                .Include(b => b.BidStaffs)
                .ThenInclude(b => b.Staff)
                .Include(b => b.Materials)
                .ThenInclude(b => b.Inventory)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (bidToUpdate == null)
            {
                return NotFound();
            }

            UpdateBidLaborStaffAndInventory(selectedOptions, selectedBidstaff, hoursWorked, InventoryID, Quantity, Price, bidToUpdate);

            if (await TryUpdateModelAsync<Bid>(bidToUpdate, "", b => b.Date, b => b.Amount, b => b.ProjectID, b => b.ApproveBidID))
            {
                try
                {
                    _context.Update(bidToUpdate);
                    await _context.SaveChangesAsync();
                    // return RedirectToAction(nameof(Details), bidToUpdate.ID);
                    return RedirectToAction(nameof(Index));
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BidExists(bidToUpdate.ID))
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
                    ModelState.AddModelError("", "Unable to Save changes. Please try again. If the probelm persist call the administrator");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }


            }
            if (!User.IsInRole("Admin"))
            {
                ViewData["ApproveBidID"] = _context.ApproveBids.FirstOrDefault(b => b.Status == "Pending").ID;
            }
            else
            {
                ViewData["ApproveBidID"] = new SelectList(_context.ApproveBids, "ID", "Status", bidToUpdate.ApproveBidID);
            }
            PopulateAssignedBidLabors(bidToUpdate);
            PopulateAssignedBidStaffs(bidToUpdate);
            PopulateDropDownList(bidToUpdate);
            return View(bidToUpdate);
        }

        // GET: Bids/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bid = await _context.Bids
                .Include(b => b.Project)
                .Include(b => b.ApproveBid)
                .Include(b => b.BidStaffs)
                .ThenInclude(b => b.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            try
            {
                _context.Bids.Remove(bid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return View(bid);
        }

        private bool BidExists(int id)
        {
            return _context.Bids.Any(e => e.ID == id);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private SelectList ProjectSelectList(Bid bid)
        {
            return new SelectList(_context.Projects, "ID", "Site", bid?.ProjectID);
        }
        private void PopulateDropDownList(Bid bid = null)
        {
            ViewData["ProjectID"] = ProjectSelectList(bid);
        }


        //method for checkboxes
        private void PopulateAssignedBidLabors(Bid bid)
        {
            var allBidLabors = _context.Labors;
            var currentLaborsID = new HashSet<int>(bid.BidLabors.Select(b => b.LaborID));
            var checkBoxes = new List<CheckBoxVM>();

            foreach (var labor in allBidLabors)
            {
                checkBoxes.Add(new CheckBoxVM
                {
                    ID = labor.ID,
                    DisplayText = labor.Type,
                    Assigned = currentLaborsID.Contains(labor.ID)
                });
            }
            ViewData["LaborOptions"] = checkBoxes;
        }
        private void UpdateBidLaborStaffAndInventory(string[] selectedOptions, string[] selectedBidstaff, string[] hoursWorked, string[] InventoryID, string[] Quantity, string[] Price, Bid bidToUpdate)
        {
            if (selectedOptions != null && hoursWorked != null)
            {
                var selectedOptionsHS = new HashSet<string>(selectedOptions);
                var bidLaborOptions = new HashSet<int>(bidToUpdate.BidLabors.Select(b => b.LaborID)); //IDS of currently selected labors

                foreach (var labor in _context.Labors)
                {
                    if (selectedOptionsHS.Contains(labor.ID.ToString()))
                    {
                        int indexOfLaborID = Array.FindIndex(selectedOptions, r => r.Contains(labor.ID.ToString()));
                        if (!bidLaborOptions.Contains(labor.ID)) //not in bidlabor table
                        {
                            bidToUpdate.BidLabors.Add(new BidLabor
                            {
                                BidID = bidToUpdate.ID,
                                LaborID = labor.ID,
                                HoursWorked = int.Parse(hoursWorked[indexOfLaborID]), //for current scenario only and will be updated when bid labor form is filled
                                ExtPrice = labor.Price * int.Parse(hoursWorked[indexOfLaborID])
                            });
                        }
                        else //update the data in bidlabor table
                        {
                            var bidLaborToUpdate = _context.BidLabors.FirstOrDefault(b => b.LaborID == labor.ID && b.BidID == bidToUpdate.ID);

                            bidLaborToUpdate.HoursWorked = int.Parse(hoursWorked[indexOfLaborID]);
                            bidLaborToUpdate.ExtPrice = labor.Price * int.Parse(hoursWorked[indexOfLaborID]);

                        }
                    }
                    else
                    {
                        //checkbox not checked
                        if (bidLaborOptions.Contains(labor.ID)) //currently in db not in the client request
                        {
                            BidLabor bidLaborToRemove = bidToUpdate.BidLabors.SingleOrDefault(b => b.LaborID == labor.ID);
                            _context.Remove(bidLaborToRemove);
                        }
                    }
                }
            }
            else
            {
                bidToUpdate.BidLabors = new List<BidLabor>();
            }
            if (selectedBidstaff != null)
            {
                var selectedOptionsHS = new HashSet<string>(selectedBidstaff);
                var bidStaffOptions = new HashSet<int>(bidToUpdate.BidStaffs.Select(b => b.StaffID)); //IDS of currently selected staffs

                foreach (var staff in _context.Staffs)
                {
                    if (selectedOptionsHS.Contains(staff.ID.ToString()))
                    {
                        if (!bidStaffOptions.Contains(staff.ID)) //not in bidstaff table
                        {
                            bidToUpdate.BidStaffs.Add(new BidStaff
                            {
                                BidID = bidToUpdate.ID,
                                StaffID = staff.ID
                            });
                        }
                    }
                    else
                    {
                        //checkbox not checked
                        if (bidStaffOptions.Contains(staff.ID)) //currently in db not in the client request
                        {
                            BidStaff bidStaffToRemove = bidToUpdate.BidStaffs.SingleOrDefault(b => b.StaffID == staff.ID);
                            _context.Remove(bidStaffToRemove);
                        }
                    }
                }
            }
            else
            {
                bidToUpdate.BidStaffs = new List<BidStaff>();
            }
            if(!(InventoryID == null && Price == null && Quantity == null))
            {
                
                var currentlySelectedInventoryID = new HashSet<int>(bidToUpdate.Materials.Select(b => b.InventoryID));

                foreach(var inventory in _context.Inventories)
                {
                    for (int i = 0; i < InventoryID.Count(); i++)
                    {
                        if (InventoryID[i] == inventory.ID.ToString())
                        {

                            if (!currentlySelectedInventoryID.Contains(inventory.ID)) //not currently selected or added
                            {

                                bidToUpdate.Materials.Add(new Material
                                {
                                    InventoryID = inventory.ID,
                                    BidID = bidToUpdate.ID,
                                    Quantity = int.Parse(Quantity[i]),
                                    Price = double.Parse(Price[i])
                                });
                            }
                            //currently selected but could be updated
                            else
                            {
                                var materialToUpdate = _context.Materials.Where(m => m.InventoryID == inventory.ID && m.BidID == bidToUpdate.ID)
                                    .FirstOrDefault();
                                materialToUpdate.Price = double.Parse(Price[i]);
                                materialToUpdate.Quantity = int.Parse(Quantity[i]);
                                
                            }
                        }
                        else
                        {
                            if (currentlySelectedInventoryID.Contains(inventory.ID)) //conditions to remove material
                            {
                                Material materialToRemove = bidToUpdate.Materials.SingleOrDefault(s => s.InventoryID == inventory.ID);
                                _context.Remove(materialToRemove);
                            }
                        }
                    }
                }
            }
            else
            {
                bidToUpdate.Materials = new List<Material>();
            }

        }
        //method of checlboxes for bid staff
        private void PopulateAssignedBidStaffs(Bid bid)
        {
            var allBidStaff = _context.Staffs.Include(s => s.Position);
            var currentStaffsID = new HashSet<int>(bid.BidStaffs.Select(b => b.StaffID));
            var checkBoxes = new List<CheckBoxVM>();

            foreach (var staff in allBidStaff)
            {
                if (staff.Position.Description == "Designer" || staff.Position.Description == "Sales Person")
                {
                    checkBoxes.Add(new CheckBoxVM
                    {
                        ID = staff.ID,
                        DisplayText = staff.FullName + ' ' + $"({staff.Position.Description})",
                        Assigned = currentStaffsID.Contains(staff.ID)
                    });
                }
            }
            ViewData["StaffOptions"] = checkBoxes;
        }

        //for maintaining the view of bid labors in bidlabor form
        [HttpGet]
        public JsonResult GetLabors(int bidID)
        {
            if (bidID != 0)
            {

                return Json(_context.BidLabors
                    .Include(b => b.Labor)
                    .Where(s => s.BidID == bidID)
                    .Select(s => new Labor
                    {
                        ID = s.LaborID,
                        Type = s.Labor.Type,
                        BidLabors = s.Labor.BidLabors
                        .Select(bl => new BidLabor
                        {
                            HoursWorked = bl.HoursWorked,
                            ExtPrice = bl.ExtPrice,
                            LaborID = bl.LaborID,
                            BidID = bl.BidID
                        })
                        .Where(bl => bl.BidID == bidID)
                        .ToList()

                    })

                     .ToArrayAsync()); ;
            }
            else
            {
                return Json(_context.Labors.ToArray());
            }
        }

        //for getting the  bid materials
        [HttpGet]
        public JsonResult GetInventoryItems(string typeID)
        {


            var inventory =  _context.Inventories
                            .Include(i => i.InventoryType)
                            .Select(s => new Inventory{
                                ID = s.ID,
                                Code = s.Code,
                                Name = s.Name,
                                Description = s.Description,
                                Size = s.Size,
                                Price = s.Price,
                                InventoryTypeID = s.InventoryTypeID                                                                     
                            })
                            .ToList();
            if (!String.IsNullOrEmpty(typeID))
            {
                inventory = inventory
                             .Where(s => s.InventoryTypeID == Convert.ToInt32(typeID))
                             .ToList();
                             
            }
            return Json(inventory);
            
        }
        [HttpGet]
        public JsonResult GetInventoryType()
        {
            return Json(_context.InventoryTypes.OrderBy(p=>p.DescOfType));
        }

        [HttpGet]
        public JsonResult GetItemByID(string ID)
        {
            return Json( _context.Inventories
                            .Include(i => i.InventoryType)
                            .Select(s => new Inventory
                            {
                                ID = s.ID,
                                Code = s.Code,
                                Name = s.Name,
                                Description = s.Description,
                                Size = s.Size,
                                Price = s.Price,
                                InventoryTypeID = s.InventoryTypeID
                            })
                            .Where(s=>s.ID == Convert.ToInt32(ID))
                            .ToList());
        }

        [HttpGet]
        public JsonResult GetBidMaterialByID(string bidID)
        {
            return Json(_context.Materials
                .Include(s => s.Inventory)
                .Select(s => new Material
                {
                    ID = s.ID,
                    InventoryID = s.InventoryID,
                    BidID = s.BidID,
                    Price = s.Price,
                    Quantity = s.Quantity,
                    Inventory = new Inventory {ID = s.Inventory.ID, Price = s.Inventory.Price, Name = s.Inventory.Name , Code =s.Inventory.Code}
                })
                .Where(s => s.BidID == int.Parse(bidID))
                .ToList());
        }

    }
}
