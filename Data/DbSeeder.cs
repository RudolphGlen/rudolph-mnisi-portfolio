using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BeginnerMvc.Models;

namespace BeginnerMvc.Data;

/// <summary>
/// Seeds the initial administrator and the starting profile and education data when missing.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Uses the application's registered managers and database context to seed startup data.
    /// </summary>
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Resolve UserManager from dependency injection so Identity can manage persistence
        // and apply its configured validation rules when it creates the account.
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var adminEmail = configuration["AdminSettings:Email"];
        var adminPassword = configuration["AdminSettings:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            throw new InvalidOperationException(
                "The admin email is missing. Set AdminSettings:Email using .NET User Secrets for local development.");
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the Admin role: {errors}");
            }
        }

        // UserManager queries the configured Identity database by email.
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "The admin password is missing. Set AdminSettings:Password using .NET User Secrets for local development.");
            }

            // Create the initial account through UserManager instead of writing Identity tables
            // directly, so the password is hashed and Identity's normal checks are applied.
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the admin user: {errors}");
            }
        }

        // Existing admin accounts may predate lockout being enabled for new users.
        // Enable it on the stored user so failed sign-ins are counted for this account too.
        if (!adminUser.LockoutEnabled)
        {
            adminUser.LockoutEnabled = true;
            var updateResult = await userManager.UpdateAsync(adminUser);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not enable admin account lockout: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not assign the admin user to the Admin role: {errors}");
            }
        }

        // Move the existing public profile defaults into the database on first startup.
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await dbContext.SiteProfiles.AnyAsync())
        {
            await dbContext.SiteProfiles.AddAsync(new SiteProfile());
        }

        // Preserve the current education cards when the new table is first created.
        if (!await dbContext.Education.AnyAsync())
        {
            await dbContext.Education.AddRangeAsync(
                new Education
                {
                    Qualification = "National Diploma in ICT",
                    Institution = "Tshwane University of Technology",
                    Dates = "Completed: February 2025",
                    DisplayOrder = 1
                },
                new Education
                {
                    Qualification = "Matric",
                    Institution = "Shanke High School",
                    Dates = "Completed: 2017",
                    DisplayOrder = 2
                });
        }

        await dbContext.SaveChangesAsync();
    }
}
