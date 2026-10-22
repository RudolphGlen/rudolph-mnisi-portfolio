using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BeginnerMvc.Data;

/// <summary>
/// Creates the initial administrator account when it is missing from the Identity store.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Uses the application's registered UserManager to find or create the admin user.
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
    }
}
