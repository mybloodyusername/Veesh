using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weesh.Domain.Entities;
using Weesh.Domain.Enums;

namespace Weesh.Infra.Data;

public static class SeedDatabase
{
    public static async Task Initialize(
        ILogger<WeeshDbContext> logger,
        IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        using var scope = serviceProvider.CreateScope();
        
        var context = serviceProvider.GetRequiredService<WeeshDbContext>();

        foreach (var role in Enum.GetValues<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(nameof(role)))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(nameof(role)));
                logger.LogInformation("Role created successfully: {RoleName}", nameof(role));
            }
            else
            {
                logger.LogInformation("Role already exists: {RoleName}", nameof(role));
            }
        }
        
        // TODO: create admins

    }
}