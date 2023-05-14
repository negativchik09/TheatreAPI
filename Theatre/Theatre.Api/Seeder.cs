using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Theatre.Domain;

namespace Theatre;

public static class Seeder
{
    public static async void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        using var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        using var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
        using var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        if (dbContext != null) await dbContext.Database.MigrateAsync();
        if (roleManager == null || userManager == null)
        {
            return;
        }

        string[] roles = { IdentityRoles.Admin, IdentityRoles.Actor };
        
        foreach (string role in roles)
        {
            if (!roleManager.Roles.Any(r => r.Name == role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (await userManager.FindByNameAsync("admin") == null)
        {
            var admin = new IdentityUser()
            {
                UserName = "admin",
                Email = "admin@gmail.com"
            };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
        }
    }
}