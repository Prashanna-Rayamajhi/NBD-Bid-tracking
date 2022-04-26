using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NBD_BID_SYSTEM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models.Controllers
{
    public static class ApplicationSeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Designer", "Manager", "SalesPerson", "User" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (userManager.FindByEmailAsync("admin1@outlook.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin1@outlook.com",
                    Email = "admin1@outlook.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "password").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            if (userManager.FindByEmailAsync("designer1@outlook.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "designer1@outlook.com",
                    Email = "designer1@outlook.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "password").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Designer").Wait();
                }
            }
            if (userManager.FindByEmailAsync("manager1@outlook.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "manager1@outlook.com",
                    Email = "manager1@outlook.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "password").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Manager").Wait();
                }
            }
            if (userManager.FindByEmailAsync("salesperson1@outlook.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "salesperson1@outlook.com",
                    Email = "salesperson1@outlook.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "password").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "SalesPerson").Wait();
                }
            }
        }
    }
}
