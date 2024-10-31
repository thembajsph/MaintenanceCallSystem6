using MaintenanceCallSystem6.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace MaintenanceCallSystem6.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            try
            {
                // Define roles
                string[] roleNames = { "Admin", "User" };

                // Ensure roles exist
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (!roleResult.Succeeded)
                        {
                            throw new Exception($"Failed to create role: {roleName}, Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                // Check if the admin user exists
                var adminEmail = "admin@example.com";
                var user = await userManager.FindByEmailAsync(adminEmail);
                if (user == null)
                {
                    // Create the admin user
                    user = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var createUserResult = await userManager.CreateAsync(user, "Admin@123");

                    if (createUserResult.Succeeded)
                    {
                        // Assign the admin role
                        var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                        if (!addRoleResult.Succeeded)
                        {
                            throw new Exception($"Failed to assign Admin role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    // Ensure the admin user has the Admin role
                    if (!await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                        if (!addRoleResult.Succeeded)
                        {
                            throw new Exception($"Failed to assign Admin role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Error during database initialization: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
//using Microsoft.Extensions.Logging;
//using Microsoft.AspNetCore.Identity;
//using MaintenanceCallSystem6.Models;
//using System;
//using System.Threading.Tasks;

//namespace MaintenanceCallSystem6.Data
//{
//    public class DbInitializer
//    {
//        public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<DbInitializer> logger)
//        {
//            string[] roleNames = { "Admin", "User" };
//            IdentityResult roleResult;

//            // Create roles if they don't exist
//            foreach (var roleName in roleNames)
//            {
//                var roleExist = await roleManager.RoleExistsAsync(roleName);
//                if (!roleExist)
//                {
//                    // Role doesn't exist, create it
//                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
//                    if (!roleResult.Succeeded)
//                    {
//                        logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join(", ", roleResult.Errors));
//                        throw new Exception("Failed to create role " + roleName + ": " + string.Join(", ", roleResult.Errors));
//                    }
//                }
//            }

//            // Create admin user if it doesn't exist
//            var adminEmail = "admin@example.com";
//            var adminPassword = "Admin@123";
//            var user = await userManager.FindByEmailAsync(adminEmail);
//            if (user == null)
//            {
//                user = new User
//                {
//                    UserName = adminEmail,
//                    Email = adminEmail,
//                    FirstName = "Admin",
//                    LastName = "User"
//                };

//                var createUserResult = await userManager.CreateAsync(user, adminPassword);
//                if (createUserResult.Succeeded)
//                {
//                    await userManager.AddToRoleAsync(user, "Admin");
//                }
//                else
//                {
//                    logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", createUserResult.Errors));
//                    throw new Exception("Failed to create admin user: " + string.Join(", ", createUserResult.Errors));
//                }
//            }
//        }
//    }
//}

