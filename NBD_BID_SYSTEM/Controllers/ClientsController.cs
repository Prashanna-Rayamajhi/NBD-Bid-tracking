using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;
using NBD_BID_SYSTEM.Utilities;

namespace NBD_BID_SYSTEM.Controllers
{
    //[Authorize]
    public class ClientsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public ClientsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string clientName, string searchCity,  string btnAction,  int? ProvinceID, int? page, int? pageSizeID, string sortDirection = "asc", string sortField = "Name")
        {
            //clearing the cookie
            CookieHelper.SetCookieOptions(HttpContext, ControllerName() + "URL", "", -1);

            var data = from d in _context.Clients
                 .Include(c => c.Province)
                 .Include(c => c.Projects)
                 .AsNoTracking()
                       select d;

            ViewData["IsFiltering"] = "";   
            //filtering the data
            if (ProvinceID.HasValue)
            {
                data = data.Where(d => d.ProvinceID == ProvinceID);
                ViewData["IsFiltering"] = "show";
            }
            if (!String.IsNullOrEmpty(clientName))
            {
                data = data.Where(d => d.Name.ToUpper().Contains(clientName.ToUpper()));
                ViewData["IsFiltering"] = "show";
            }
            if (!String.IsNullOrEmpty(searchCity))
            {
                data = data.Where(d => d.City.ToUpper().Contains(searchCity.ToUpper()));
                ViewData["IsFiltering"] = "show";
            }
            //Sorting the fields
            //Array of SortFields
            string[] possibleSortFileds = new[] { "Name", "City", "Province", "Contact Person" };
            //Matching the condition of sort direction and sortField
            if(possibleSortFileds.Contains(btnAction)) //submit is done by sortFields
            {
                page = 1; //resetting the page to 1 when sortin is applied
                if (sortField == btnAction)
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                sortField = btnAction;

            }
            data = sortField switch
            {
                "City" => sortDirection == "asc" ? data.OrderBy(d => d.City) : data.OrderByDescending(d => d.City),
                "Province" => sortDirection == "asc" ? data.OrderBy(d => d.Province.Abbrevation) : data.OrderByDescending(d => d.Province.Abbrevation),
                "Contact Person" => sortDirection =="asc"? data.OrderBy(d => d.CpLName).ThenBy(d => d.CpFName) : data.OrderByDescending(d => d.CpLName).ThenByDescending(d => d.CpFName),
                _ => sortDirection == "asc" ? data.OrderBy(d => d.Name) : data.OrderByDescending(d => d.Name),
            };
            ViewData["sortDirection"] = sortDirection;
            ViewData["sortField"] = sortField;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Client>.CreateAsync(data.AsNoTracking(), page ?? 1, pageSize);  

            PopulateDropDownList();
            return View(pagedData);
        }



        // GET: Clients/Details/5
        //[Authorize(Roles = "Admin,Designer,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Province)
                .Include(c => c.Projects)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        //[Authorize(Roles = "Admin,Designer")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            Client client = new Client
            {
                ProvinceID = _context.Provinces.FirstOrDefault(p => p.Name == "Ontario").ID
            };

            PopulateDropDownList(client);
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Designer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,CpFName,CpLName,CpPosition,Address,City,ProvinceID,PostalCode,PhoneNumber")] Client client)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { client.ID});
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownList(client);
            return View(client);
        }

        // GET: Clients/Edit/5
        //[Authorize(Roles = "Admin,Designer")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            PopulateDropDownList(client);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Designer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Go get the client to update
            var clientToUpdate = await _context.Clients.FirstOrDefaultAsync(p => p.ID == id);

            //Check that you got it or exit with a not found error
            if (clientToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Client>(clientToUpdate, "",
                p => p.Name, p => p.CpFName, p => p.CpLName, p => p.CpPosition, p => p.Address,
                p => p.City, p => p.ProvinceID, p => p.PostalCode, p => p.PhoneNumber))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { clientToUpdate.ID});
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(clientToUpdate.ID))
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
            PopulateDropDownList(clientToUpdate);
            return View(clientToUpdate);
        }

        // GET: Clients/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Province)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var client = await _context.Clients.FindAsync(id);
            try
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Client. Remember, you cannot delete a Client that has projects assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(client);
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ID == id);
        }

        //method to populate the select list
        private void PopulateDropDownList(Client client = null)
        {
            var selectList = from p in _context.Provinces
                             orderby p.Name
                             select p;

            ViewData["ProvinceID"] = new SelectList(selectList, "ID", "ProvinceFormatted", client?.ProvinceID);
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
