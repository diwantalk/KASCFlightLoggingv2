using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using KASCFlightLogging.Models; // Adjust namespace if necessary

namespace KASCFlightLogging.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Staff", "User", "Pilot" };

            foreach (var roleName in roleNames)
            {
                // Check if the role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create an admin user if it doesn't exist
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Use a secure password

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Optionally, create staff user
            var staffEmail = "staff@example.com";
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(staffUser, "Staff@123"); // Use a secure password

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
            }

            // Add more users and roles as needed
        }
    }
}