using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stackbuld.Assessment.CSharp.Domain.Constants;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Stackbuld.Assessment.CSharp.Infrastructure.Persistence.DbContexts;

namespace Stackbuld.Assessment.CSharp.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        // Ensure roles exist
        foreach (var role in Roles.List)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // Seed an admin user
        var admins = await userManager.GetUsersInRoleAsync(Roles.Admin);
        if (admins.Count == 0)
        {
            var adminEmail = "admin@example.com";
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "Test",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedBy = "Seeder",
                UpdatedBy = "Seeder",
            };

            var result = await userManager.CreateAsync(admin, "Admin@123"); // secure password
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
            else
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Seed a merchant
        var merchants = await userManager.GetUsersInRoleAsync(Roles.Merchant);
        if (merchants.Count == 0)
        {
            var merchantEmail = "merchant@example.com";
            var merchant = new User
            {
                FirstName = "Merchant",
                LastName = "Test",
                UserName = merchantEmail,
                Email = merchantEmail,
                EmailConfirmed = true,
                CreatedBy = "Seeder",
                UpdatedBy = "Seeder",
            };

            var result = await userManager.CreateAsync(merchant, "Merchant@123"); // secure password
            if (result.Succeeded)
            {
                await dbContext.Merchants.AddAsync(
                    new Merchant { BusinessName = "Test Business", UserId = merchant.Id });
                await userManager.AddToRoleAsync(merchant, Roles.Merchant);
            }
            else
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Seed a user
        var users = await userManager.GetUsersInRoleAsync(Roles.User);
        if (users.Count == 0)
        {
            var userEmail = "user@example.com";
            var user = new User
            {
                FirstName = "User",
                LastName = "Test",
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                CreatedBy = "Seeder",
                UpdatedBy = "Seeder",
            };

            var result = await userManager.CreateAsync(user, "User@123"); // secure password
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Roles.User);
            }
            else
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}