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
    public class ProjectsController : Controller
    {
        private readonly NBDBidSystemContext _context;

        public ProjectsController(NBDBidSystemContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string clientName, string projectSite, string bidDate, string btnAction, int? page, int? pageSizeID,string sortField = "Site", string sortDirection = "asc")
        {
            //clearing the cookie
            CookieHelper.SetCookieOptions(HttpContext, ControllerName() + "URL", "", -1);
            var projects = from p in _context.Projects
                .Include(p => p.Client)
                .Include(p=> p.Bids)
                .AsNoTracking()
                select p;
            ViewData["IsFiltering"] = "";

            if (!String.IsNullOrEmpty(clientName))
            {
                projects = projects.Where(p => p.Client.Name.ToUpper().Contains(clientName.ToUpper()));
                ViewData["IsFiltering"] = "show";
            }
            if (!String.IsNullOrEmpty(projectSite))
            {
                projects = projects.Where(p => p.Site.ToUpper().Contains(projectSite.ToUpper()));
                ViewData["IsFiltering"] = "show";
            }
            if (!String.IsNullOrEmpty(bidDate))
            {
                projects = projects.Where(p => p.Bids.FirstOrDefault().Date == DateTime.Parse(bidDate));
                ViewData["IsFiltering"] = "show";
            }
            //for sorting of data
            if(btnAction != "Filter" && !String.IsNullOrEmpty(btnAction)) //request made by sorting fields
            {
                if (sortField == btnAction) //request thorugh default sortfield
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                sortField = btnAction;
            }
            switch (sortField) 
            {
                case "Begin Date":
                    projects = sortDirection == "asc" ? projects.OrderBy(p => p.BeginDate.Year).ThenBy(p => p.BeginDate.Month).ThenBy(p => p.BeginDate.Day) : projects.OrderByDescending(p => p.BeginDate.Year).ThenByDescending(p => p.BeginDate.Month).ThenByDescending(p => p.BeginDate.Day);
                    break;
                case "Completion Date":
                    projects = sortDirection == "asc" ? projects.OrderBy(p => p.CompletionDate.Year).ThenBy(p => p.CompletionDate.Month).ThenBy(p => p.CompletionDate.Day) : projects.OrderByDescending(p => p.CompletionDate.Year).ThenByDescending(p => p.CompletionDate.Month).ThenByDescending(p => p.BeginDate.Day);
                    break;
                case "Bid Date":
                    projects = sortDirection == "asc" ? projects.OrderBy(p => p.Bids.FirstOrDefault().Date.Year).ThenBy(p => p.Bids.FirstOrDefault().Date.Month).ThenBy(p => p.Bids.FirstOrDefault().Date.Day) : projects.OrderByDescending(p => p.Bids.FirstOrDefault().Date.Year).ThenByDescending(p => p.Bids.FirstOrDefault().Date.Month).ThenByDescending(p => p.Bids.FirstOrDefault().Date.Day);
                    break;
                case "Client":
                    projects = sortDirection == "asc" ? projects.OrderBy(p => p.Client.Name) : projects.OrderByDescending(p => p.Client.Name);
                    break;
                default:
                    projects = sortDirection == "asc" ? projects.OrderBy(p => p.Site) : projects.OrderByDescending(p => p.Site);
                    break;
            }
            ViewData["sortDirection"] = sortDirection;
            ViewData["sortField"] = sortField;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Projects/Details/5
        //[Authorize(Roles = "Admin,Designer,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p=> p.Bids)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        //[Authorize(Roles = "Admin,Designer")]
        public IActionResult Create(int? ClientID)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Sets bid date default to today
            Project project = new Project();
           

            //Method added to change client address to client name in drop down lists on create and edit views
            if (!ClientID.HasValue)
            {
                PopulateDropDownLists();
            }
            else{
                ViewData["ClientID"] = new SelectList(_context.Clients.OrderBy(s=>s.Name), "ID", "Name", ClientID);
            }
            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Designer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Site,BeginDate,CompletionDate,ClientID")] Project project)
        {
            ViewDataReturnURL();
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { project.ID});
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            //Method added to change client address to client name in drop down lists on create and edit views
            PopulateDropDownLists(project);
            return View(project);
        }

        // GET: Projects/Edit/5
        //[Authorize(Roles = "Admin,Designer")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewDataReturnURL();
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            //Method added to change client address to client name in drop down lists on create and edit views
            PopulateDropDownLists(project);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin,Designer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            ViewDataReturnURL();
            //Go get the Project to update
            var projectToUpdate = await _context.Projects.FirstOrDefaultAsync(p => p.ID == id);

            //Check that you got it or exit with a not found error
            if (projectToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Project>(projectToUpdate, "",
                p => p.Site, p => p.BeginDate, p => p.CompletionDate, p => p.ClientID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { projectToUpdate.ID});
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(projectToUpdate.ID))
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
            //Method added to change client address to client name in drop down lists on create and edit views
            PopulateDropDownLists(projectToUpdate);
            return View(projectToUpdate);
        }

        // GET: Projects/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Bids)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewDataReturnURL();
            var project = await _context.Projects.FindAsync(id);
            try
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return View(project);
        }

        //Method added to change client address to client name in drop down lists on create and edit views
        private void PopulateDropDownLists(Project project = null)
        {
            var dQuery = from c in _context.Clients
                         orderby c.Name
                         select c;
            ViewData["ClientID"] = new SelectList(dQuery, "ID", "Name", project?.ClientID);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ID == id);
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
