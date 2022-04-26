using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NBD_BID_SYSTEM.Data;
using NBD_BID_SYSTEM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly NBDBidSystemContext _context;
        public HomeController(ILogger<HomeController> logger, NBDBidSystemContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var data =  await _context.Projects
                       .Include(c => c.Client)
                       .Include(c => c.Bids)
                       .ThenInclude(c => c.ApproveBid)
                       .Include(c => c.Bids)
                       .ThenInclude(c => c.BidStaffs)
                       .ThenInclude(c => c.Staff) 
                       .AsNoTracking()
                       .ToListAsync();
            //getting data for the chart for admin
            if (User.Identity.IsAuthenticated)
            {
                var bidData = await _context.Bids?.Include(b => b.ApproveBid)
                                   .Include(b => b.Project)
                                   .Select(grp => new
                                   {
                                       Name = grp.Project.Site,
                                       Amount = grp.Amount
                                   })
                                   .ToListAsync();
                //calculating total amount in all approved bids by client
                var approvedBidsByComp = _context.Bids?.Include(b => b.ApproveBid)
                                        .Include(b => b.BidLabors)
                                        .ThenInclude(b => b.Labor)
                                        .Include(b=> b.Materials)
                                        .ThenInclude( b=> b.Inventory)
                                .Where(b => b.ApproveBid.Status == "Approved By Company" || b.ApproveBid.Status == "Approved By Client")
                                .ToArray();

                var totalAmt = approvedBidsByComp.Sum(s => s.Amount);
                double expForLabor = 0;
                foreach (var bid in approvedBidsByComp)
                {
                    expForLabor += bid.BidLabors.Sum(s => s.ExtPrice);
                }

                double expForMaterial = 0;
                foreach (var bid in approvedBidsByComp)
                {
                    expForLabor += bid.Materials.Sum(s => s.Price);
                }
                var totalExp = (expForLabor + expForMaterial);
                var profit = totalAmt - totalExp;
                                

                //data for chart for other user
                var loggedInStaff = _context.Staffs
                                    .FirstOrDefault(s => s.Email == User.Identity.Name);

                var bids = await _context.Bids?.Include(b => b.ApproveBid)
                                   .Include(b => b.Project)
                                   .Include(b => b.BidStaffs)
                                   .ThenInclude(b => b.Staff)
                                   .Where(b => b.BidStaffs.FirstOrDefault().StaffID == loggedInStaff.ID)
                                   .ToListAsync();
                var bidSummary = bids.Select(grp => new
                {
                    Name = grp.Project.Site,
                    Amount = grp.Amount
                });
                var approvedBid = bids
                                 .Where(b => b.ApproveBid.Status != "Rejected" || b.ApproveBid.Status != "Pending")
                                 .Count();


                if (User.IsInRole("Admin") || User.IsInRole("Manager"))
                {
                    var info = JsonConvert.SerializeObject(bidData);
                    ViewData["BidData"] = info;
                    ViewData["TotalBidAmount"] = totalAmt;
                    ViewData["TotalExpense"] = totalExp;
                    ViewData["Profit"] = profit;

                }
                else if(User.IsInRole("Designer"))
                {
                    var info = JsonConvert.SerializeObject(bidSummary);
                    ViewData["BidData"] = info;
                    ViewData["TotalBids"] = bids.Count();
                    ViewData["Approved"] = approvedBid;
                }
                else
                {
                    var summary = _context.Clients.Include(c => c.Projects).Select(grp => new
                    {
                        Client = grp.Name,
                        Projects = grp.Projects.Count()
                    });
                    
                    var summaryData = JsonConvert.SerializeObject(summary); 
                    ViewData["SummaryData"] = summaryData;

                }
            }

            return View(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
