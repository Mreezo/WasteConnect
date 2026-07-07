using Microsoft.AspNetCore.Identity;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public static class SeedService
    {
        public static async Task SeedRolesAndAdminAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "User",
                "Company",
                "Admin"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            string adminEmail = "admin@wasteconnect.co.za";

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = "System Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
                    PhoneNumber = "0712345678",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser,
                    "Admin@12345");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");
                }
            }
        }
    }
}