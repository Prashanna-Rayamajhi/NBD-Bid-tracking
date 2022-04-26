using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NBD_BID_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Data
{
    public static class NBDSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new NBDBidSystemContext(serviceProvider.GetRequiredService<DbContextOptions<NBDBidSystemContext>>()))
            {
                if (!context.Provinces.Any())
                {
                    context.AddRange(
                        new Province
                        {
                            Name = "Ontario",
                            Abbrevation = "ON"
                        },
                        new Province
                        {
                            Name = "Newfoundland and Labrador",
                            Abbrevation = "NL"
                        },
                        new Province
                        {
                            Name = "Prince Edward Island",
                            Abbrevation = "PE"
                        },
                        new Province
                        {
                            Name = "Nova Scotia",
                            Abbrevation = "NS"
                        },
                        new Province
                        {
                            Name = "New Brunswick",
                            Abbrevation = "NB"
                        },
                        new Province
                        {
                            Name = "Manitoba",
                            Abbrevation = "MB"
                        },
                        new Province
                        {
                            Name = "Saskatchewan",
                            Abbrevation = "SK"
                        },
                        new Province
                        {
                            Name = "Quebec",
                            Abbrevation = "QC"
                        },
                        new Province
                        {
                            Name = "Alberta",
                            Abbrevation = "AB"
                        },
                        new Province
                        {
                            Name = "British Columbia",
                            Abbrevation = "BC"
                        },
                        new Province
                        {
                            Name = "Yukon",
                            Abbrevation = "YT"
                        },
                        new Province
                        {
                            Name = "Northwest Territories",
                            Abbrevation = "NT"
                        },
                        new Province
                        {
                            Name = "Nunavut",
                            Abbrevation = "NU"
                        }
                        );
                    context.SaveChanges();
                }
                string[] clientNames = { "City Center", "Niagara College", "Fair View Mall", "Stella Colony", "Holiday Inn" };
                if (!context.Clients.Any())
                {
                    context.Clients.AddRange(
                        new Client{
                            Name = clientNames[0],
                            CpFName = "Andrew",
                            CpLName = "Watson",
                            CpPosition = "Manager",
                            Address = "4310 Queen St.",
                            City = "Niagara Falls",
                            ProvinceID = context.Provinces.FirstOrDefault().ID,
                            PostalCode = "L2E 6X5",
                            PhoneNumber = "9053567521"
                        },
                        new Client
                        {
                            Name = clientNames[1],
                            CpFName = "Emmanule",
                            CpLName = "Watt",
                            CpPosition = "General Manager",
                            Address = "100 Niagara College Boulevard",
                            City="Welland",
                            ProvinceID = context.Provinces.FirstOrDefault().ID,
                            PostalCode = "L3C 7L3",
                            PhoneNumber = "9057352211"
                        },
                        new Client
                        {
                            Name = clientNames[2],
                            CpFName = "Andy",
                            CpLName = "Mondello",
                            CpPosition = "Senior Property Manager",
                            Address = "285 Geneva Street",
                            City = "St.Catherines",
                            ProvinceID = context.Provinces.FirstOrDefault().ID,
                            PostalCode = "L2N 2G1",
                            PhoneNumber = "9058269447"
                        },
                         new Client
                         {
                             Name = clientNames[3],
                             CpFName = "Anna",
                             CpLName = "Smith",
                             CpPosition = "Property Manager",
                             Address = "205 Geneva Street",
                             City = "St.Catherines",
                             ProvinceID = context.Provinces.FirstOrDefault().ID,
                             PostalCode = "L2N 2G1",
                             PhoneNumber = "9058269441"
                         },
                          new Client
                          {
                              Name = clientNames[4],
                              CpFName = "Tom",
                              CpLName = "Jackson",
                              CpPosition = "Regional Manager",
                              Address = "5339 Murray St",
                              City = "Niagara Falls",
                              ProvinceID = context.Provinces.FirstOrDefault().ID,
                              PostalCode = "L2G 2J3",
                              PhoneNumber = "9053561333"
                          }
                        );
                    context.SaveChanges();
                        
                }
                if (!context.Projects.Any())
                {
                    context.Projects.AddRange(
                        new Project {
                            Site = "City Hall",
                            BeginDate = DateTime.Parse("2021-12-17"),
                            CompletionDate = DateTime.Parse("2022-01-17"),
                            
                            ClientID = context.Clients.FirstOrDefault(p=> p.Name == clientNames[0]).ID
                        },
                        new Project
                        {
                            Site = "Library",
                            BeginDate = DateTime.Parse("2022-01-10"),
                            CompletionDate = DateTime.Parse("2022-02-15"),
                           
                            ClientID = context.Clients.FirstOrDefault(p=> p.Name == clientNames[1]).ID
                        },
                        new Project
                        {
                            Site = "Lobby",
                            BeginDate = DateTime.Parse("2022-02-15"),
                            CompletionDate = DateTime.Parse("2022-02-28"),
                            
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[0]).ID
                        },
                        new Project 
                        {
                            Site = "Entrance Route",
                            BeginDate = DateTime.Parse("2022-02-15"),
                            CompletionDate = DateTime.Parse("2022-02-28"),
                          
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[2]).ID
                        },
                        new Project
                        {
                            Site = "Waiting Hall",
                            BeginDate = DateTime.Parse("2021-11-01"),
                            CompletionDate = DateTime.Parse("2021-11-17"),
                           
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[3]).ID
                        },
                        new Project 
                        {
                            Site = "Cafeteria",
                            BeginDate = DateTime.Parse("2022-01-15"),
                            CompletionDate = DateTime.Parse("2022-01-31"),
                           
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[1]).ID
                        },
                        new Project
                        {
                            Site = "Admin Room",
                            BeginDate = DateTime.Parse("2022-02-11"),
                            CompletionDate = DateTime.Parse("2022-02-16"),
                            
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[2]).ID
                        },
                        new Project
                        {
                            Site = "Garden",
                            BeginDate = DateTime.Parse("2022-01-25"),
                            CompletionDate = DateTime.Parse("2022-03-15"),

                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[3]).ID
                        },
                        new Project
                        {
                            Site = "Reception and Waiting Hall",
                            BeginDate = DateTime.Parse("2022-01-25"),
                            CompletionDate = DateTime.Parse("2022-03-15"),

                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[4]).ID
                        },
                        new Project
                        {
                            Site = "Conference Hall",
                            BeginDate = DateTime.Parse("2021-12-25"),
                            CompletionDate = DateTime.Parse("2022-02-15"),
                            
                            ClientID = context.Clients.FirstOrDefault(p => p.Name == clientNames[4]).ID
                        }

                        );
                    context.SaveChanges();
                }
                if (!context.ApproveBids.Any())
                {
                    context.ApproveBids.AddRange(
                        new ApproveBid
                        {
                            Status = "Approved By Company"
                        },
                        new ApproveBid
                        {
                            Status = "Approved By Client"
                        },
                         new ApproveBid
                         {
                             Status = "Pending"
                         },
                          new ApproveBid
                          {
                              Status = "Rejected"
                          }
                        );
                    context.SaveChanges();
                }

                string[] bidDates = new[] { "2021-12-25", "2020-12-11", "2021-12-12", "2021-06-12", "2022-01-01", "2021-06-24", "2021-05-03", "2021-11-11", "2022-02-12" };
                string[] sites = new[] { "City Hall", "Library", "Lobby", "Entrance Route", "Waiting Hall", "Cafeteria", "Admin Room", "Garden", "Reception and Waiting Hall", "Conference Hall"};
                Random random = new Random();
                if (!context.Bids.Any())
                {
                    var approveBids = context.ApproveBids.ToArray();
                    foreach(string site in sites)
                    {

                        Bid b = new Bid
                        {
                            Date = DateTime.Parse(bidDates[random.Next(0, bidDates.Length)]),
                            Amount = double.Parse((random.NextDouble() * 20000 + 1000).ToString("#.##")),
                            ProjectID = context.Projects.FirstOrDefault(d => d.Site == site).ID,
                            ApproveBidID = approveBids[random.Next(0, approveBids.Count())].ID
                        };

                        
                       context.Bids.Add(b);
                    }
                    context.SaveChanges();
                   
                }
                if (!context.Labors.Any())
                {
                    context.AddRange(
                        new Labor
                        {
                            Type = "Production worker",
                            Price = 30.00,
                            Cost = 18.00
                        },
                        new Labor
                        {
                            Type = "Designer",
                            Price = 65.00,
                            Cost = 40.00
                        },
                        new Labor
                        {
                            Type = "Equipment operator",
                            Price = 65.00,
                            Cost = 45.00
                        },
                        new Labor
                        {
                            Type = "Botanist",
                            Price = 75.00,
                            Cost = 50.00
                        }
                        );
                    context.SaveChanges();
                }
                if (!context.InventoryTypes.Any())
                {
                    context.AddRange(
                        new InventoryType
                        {
                            DescOfType = "Plant"
                        },
                        new InventoryType
                        {
                            DescOfType = "Material"
                        },
                        new InventoryType
                        {
                            DescOfType = "Pottery"
                        }
                        );
                    context.SaveChanges();
                }
                if (!context.Inventories.Any())
                {
                    context.AddRange(
                        new Inventory
                        {
                            Code = "lacco",
                            Name = "lacco australasica",
                            Description = "lacco australasica",
                            Size = "15 gal",
                            Price = 749.00,
                            InventoryTypeID = 1
                        },
                        new Inventory
                        {
                            Code = "arenga",
                            Name = "arenga pinnata",
                            Description = "arenga pinnata",
                            Size = "15 gal",
                            Price = 516.00,
                            InventoryTypeID = 1
                        },
                        new Inventory
                        {
                            Code = "CBRK5",
                            Name = "decorative cedar bark",
                            Description = "decorative cedar bark",
                            Size = "bag (5 cu ft)",
                            Price = 15.95,
                            InventoryTypeID = 2
                        },
                        new Inventory
                        {
                            Code = "CRGRN",
                            Name = "crushed granite",
                            Description = "crushed granite",
                            Size = "yard",
                            Price = 14.00,
                            InventoryTypeID = 2
                        },
                        new Inventory
                        {
                            Code = "TCP50",
                            Name = "t/c pot",
                            Description = "t/c pot",
                            Size = "50 gal",
                            Price = 110.95,
                            InventoryTypeID = 3
                        },
                        new Inventory
                        {
                            Code = "GP50",
                            Name = "granite pot",
                            Description = "granite pot",
                            Size = "50 gal",
                            Price = 195.00,
                            InventoryTypeID = 3
                        }
                        );
                    context.SaveChanges();
                }
                if (!context.Materials.Any())
                {
                    context.AddRange(
                        new Material
                        {
                            Price = 195.00,
                            Quantity = 1,
                            BidID = 1,
                            InventoryID = 1
                        },
                        new Material
                        {
                            Price = 28.00,
                            Quantity = 2,
                            BidID = 1,
                            InventoryID = 3
                        },
                        new Material
                        {
                            Price = 110.95,
                            Quantity = 1,
                            BidID = 2,
                            InventoryID = 2
                        },
                        new Material
                        {
                            Price = 31.90,
                            Quantity = 2,
                            BidID = 2,
                            InventoryID = 4
                        },
                        new Material
                        {
                            Price = 195.00,
                            Quantity = 1,
                            BidID = 3,
                            InventoryID = 1
                        },
                        new Material
                        {
                            Price = 1032.00,
                            Quantity = 2,
                            BidID = 3,
                            InventoryID = 5
                        },
                        new Material
                        {
                            Price = 14.00,
                            Quantity = 1,
                            BidID = 4,
                            InventoryID = 3
                        },
                        new Material
                        {
                            Price = 1498.00,
                            Quantity = 2,
                            BidID = 4,
                            InventoryID = 6
                        },
                        new Material
                        {
                            Price = 31.90,
                            Quantity = 2,
                            BidID = 5,
                            InventoryID = 4
                        },
                        new Material
                        {
                            Price = 195.00,
                            Quantity = 1,
                            BidID = 5,
                            InventoryID = 1
                        },
                        new Material
                        {
                            Price = 28.00,
                            Quantity = 2,
                            BidID = 6,
                            InventoryID = 3
                        },
                        new Material
                        {
                            Price = 110.95,
                            Quantity = 1,
                            BidID = 6,
                            InventoryID = 2
                        },
                        new Material
                        {
                            Price = 31.90,
                            Quantity = 2,
                            BidID = 7,
                            InventoryID = 4
                        },
                        new Material
                        {
                            Price = 195.00,
                            Quantity = 1,
                            BidID = 7,
                            InventoryID = 1
                        },
                        new Material
                        {
                            Price = 1032.00,
                            Quantity = 2,
                            BidID = 8,
                            InventoryID = 5
                        },
                        new Material
                        {
                            Price = 14.00,
                            Quantity = 1,
                            BidID = 8,
                            InventoryID = 3
                        },
                        new Material
                        {
                            Price = 1498.00,
                            Quantity = 2,
                            BidID = 9,
                            InventoryID = 6
                        },
                        new Material
                        {
                            Price = 31.90,
                            Quantity = 2,
                            BidID = 9,
                            InventoryID = 4
                        },
                        new Material
                        {
                            Price = 195.00,
                            Quantity = 1,
                            BidID = 10,
                            InventoryID = 1
                        },
                        new Material
                        {
                            Price = 28.00,
                            Quantity = 2,
                            BidID = 10,
                            InventoryID = 3
                        }
                        );
                    context.SaveChanges();
                }
                Random radLabor = new Random();
                var bidsFromDb = context.Bids.ToArray();
                var laborsFromDb = context.Labors.ToArray();
                if (!context.BidLabors.Any())
                {
                    if (bidsFromDb.Count() > 0)
                    {
                        foreach (var bid in bidsFromDb)
                        {
                          
                           foreach(var labor in laborsFromDb)
                            {
                                if(radLabor.Next(laborsFromDb.Count()) <= radLabor.Next(laborsFromDb.Count()))
                                {
                                    int hrs = radLabor.Next(1, 10);
                                    BidLabor bl = new BidLabor
                                    {
                                        BidID = bid.ID,
                                        LaborID = labor.ID,
                                        HoursWorked = hrs, 
                                        ExtPrice = hrs * labor.Price

                                    };
                                    context.BidLabors.Add(bl);
                                }
                                
                            }
                            context.SaveChanges();

                        }
                       
                    }
                }
                string[] positions = new[] { "Designer", "Production Worker", "Botanist", "Sales Person", "Admin", "Manager" };
                //working on postition
                if (!context.Positions.Any())
                {

                    foreach(var position in positions)
                    {
                        Position p = new Position
                        {
                            Description = position
                        };
                        context.Add(p);
                    }
                    context.SaveChanges();
                }
                //working on staff
                if (!context.Staffs.Any())
                {
                    context.Staffs.AddRange(
                        new Staff {
                            FirstName = "Cheryl",
                            LastName = "Poy",
                            Email = "cheryl@outlook.com",
                            PositionID = context.Positions.FirstOrDefault(p => p.Description == "Admin").ID,
                            Phone = "6476478381"
                        },
                        new Staff
                        {
                            FirstName = "Connie",
                            LastName = "Nyugen",
                            Email = "connie@outlook.com",
                            PositionID = context.Positions.FirstOrDefault(p => p.Description == "Admin").ID,
                            Phone = "6476175681"
                        },
                        new Staff
                        {
                            FirstName = "Keri",
                            LastName = "Yamaguchi",
                            Email = "keri@outlook.com",
                            PositionID = context.Positions.FirstOrDefault(p => p.Description == "Designer").ID,
                            Phone = "9051175622"
                        },
                        new Staff 
                        {
                            FirstName = "Stan",
                            LastName = "Fenton",
                            Email = "stan@outlook.com",
                            PositionID = context.Positions.FirstOrDefault(p => p.Description == "Manager").ID,
                            Phone = "9051072782"
                        },
                         new Staff
                         {
                             FirstName = "Bod",
                             LastName = "Reinhardt",
                             Email = "bob@outlook.com",
                             PositionID = context.Positions.FirstOrDefault(p => p.Description == "Sales Person").ID,
                             Phone = "9051172889"
                         },
                         new Staff
                         {
                             FirstName = "Tamara",
                             LastName = "Bakken",
                             Email = "tamara@outlook.com",
                             PositionID = context.Positions.FirstOrDefault(p => p.Description == "Designer").ID,
                             Phone = "9051172889"
                         }

                        );
                    context.SaveChanges();
                }
                if (context.Staffs.Count() > 0)
                {
                    try
                    {
                        var staffs = context.Staffs.Include(s => s.Position).ToArray();
                        var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        var allRoles = RoleManager.Roles.ToArray();
                       
                        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                        foreach (var staff in staffs)
                        {
                            bool staffAddedToRole = false;
                            foreach(var role in allRoles)
                            {
                                if (role.Name.ToLower() == staff.Position.Description.ToLower()) 
                                {
                                    staffAddedToRole = true;
                                    if (userManager.FindByEmailAsync(staff.Email.ToString()).Result == null)
                                    {
                                        IdentityUser user = new IdentityUser
                                        {
                                            UserName = staff.Email,
                                            Email = staff.Email
                                        };
                                        IdentityResult result = userManager.CreateAsync(user, "password").Result;
                                        if (result.Succeeded)
                                        {
                                            userManager.AddToRoleAsync(user, role.Name).Wait();
                                        }
                                    }

                                }
                              
                            }
                            if (!staffAddedToRole)
                            {
                                if (userManager.FindByEmailAsync(staff.Email.ToString()).Result == null)
                                {
                                    IdentityUser user = new IdentityUser
                                    {
                                        UserName = staff.Email,
                                        Email = staff.Email
                                    };

                                    IdentityResult result = userManager.CreateAsync(user, "password").Result;
                                    if (result.Succeeded)
                                    {
                                        userManager.AddToRoleAsync(user, "User").Wait();
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception("Couldnot complete task");
                    }
                }
                if (!context.BidStaffs.Any())
                {
                    var allBids = context.Bids.ToArray();
                    foreach(var bid in allBids)
                    {
                        var allStaff = context.Staffs.Include(s => s.Position).ToArray();
                        for (int i = 0; i < 2; i++)
                        {
                            Staff s = allStaff[random.Next(1, allStaff.Count())];
                            BidStaff b = new BidStaff
                            {
                                BidID = bid.ID,
                                StaffID = s.ID
                            };
                            context.BidStaffs.Add(b);

                        }

                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
