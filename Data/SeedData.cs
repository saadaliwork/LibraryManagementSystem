using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            using (var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>())
            using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
            {
                // Create roles
                string[] roleNames = { "Admin", "Member" };
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create admin user
                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Create member user
                var memberEmail = "member@example.com";
                var memberUser = await userManager.FindByEmailAsync(memberEmail);
                if (memberUser == null)
                {
                    memberUser = new IdentityUser { UserName = memberEmail, Email = memberEmail };
                    var result = await userManager.CreateAsync(memberUser, "Member@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(memberUser, "Member");
                    }
                }

                // Add test book
                if (!context.Books.Any())
                {
                    context.Books.Add(new Book
                    {
                        Title = "Test Book",
                        Author = "Test Author",
                        PublishedYear = 2023,
                        Price = 29.99m,
                        Genre = "Fiction"
                    });
                    await context.SaveChangesAsync();
                }

                await context.SaveChangesAsync();
            }
        }
    }
}