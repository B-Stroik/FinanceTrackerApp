using Microsoft.AspNetCore.Identity;

namespace FinanceTrackerApi.Data;

public static class DbInitializer
{
    public static async Task SeedUsersAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        const string userRole = "User";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(new IdentityRole(userRole));
        }

        await EnsureUserAsync(userManager, "admin@financetracker.local", "Admin123$", adminRole);
        await EnsureUserAsync(userManager, "user@financetracker.local", "User123$", userRole);
    }

    private static async Task EnsureUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create seeded user '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
