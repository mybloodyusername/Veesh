using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Infra.Data;

public static class SeedDatabase
{
    public static async Task Initialize(
        ILogger<VeeshDbContext> logger,
        IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        using var scope = serviceProvider.CreateScope();

        var context = serviceProvider.GetRequiredService<VeeshDbContext>();

        foreach (var role in Enum.GetValues<UserRole>().Select(x => x.ToString()))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                logger.LogInformation("Role created successfully: {RoleName}", role);
            }
            else
            {
                logger.LogInformation("Role already exists: {RoleName}", role);
            }
        }

        // TODO: create admins

        var admin = new
        {
            Email = "admin@veesh.com",
            Password = "Admin@123",
            Name = "Administrator",
            UserName = "admin"
        };

        var adminUser = await userManager.FindByEmailAsync(admin.Email);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Email = admin.Email,
                Name = admin.Name,
                UserName = admin.UserName,
                BirthDate = DateTimeOffset.UtcNow,
                Bio = "The very first admin of this web application.",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(adminUser, admin.Password);
            if (result.Succeeded) logger.LogInformation("Admin created successfully: {UserName}", admin.Email);
            else
                throw new Exception(
                    $"Admin creation failed: {string.Join(",", result.Errors.Select(e => e.Description))}");
        }
        else
        {
            logger.LogInformation("Admin exists: {Email}", admin.Email);
        }

        var roles = await userManager.GetRolesAsync(adminUser);
        if (roles.Contains(nameof(UserRole.Admin)))
            logger.LogInformation("Admin has role: {RoleName}", nameof(UserRole.Admin));
        else
        {
            var result = await userManager.AddToRoleAsync(adminUser, nameof(UserRole.Admin));
            if (result.Succeeded)
                logger.LogInformation("Admin role assigned successfully: {Role}", UserRole.Admin);
            else
                throw new Exception(
                    $"Admin role assignment failed: {string.Join(",", result.Errors.Select(e => e.Description))}");
        }
    }
}