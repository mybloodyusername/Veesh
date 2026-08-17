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
            Email = "admin@weesh.com",
            Password = "Admin@123",
            Name = "Administrator",
            UserName ="admin"
        };

        if (await userManager.FindByEmailAsync(admin.Email) == null)
        {
            var result = await userManager.CreateAsync(new ApplicationUser
            {
                Email = admin.Email,
                Name = admin.Name,
                UserName = admin.UserName,
                BirthDate = DateTimeOffset.UtcNow,
                Bio = "The very first admin of this web application.",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                EmailConfirmed =  true,
            }, admin.Password);
            if (result.Succeeded) logger.LogInformation("Admin created successfully: {UserName}", admin.Email);
            else
                throw new Exception(
                    $"Admin creation failed: {string.Join(",", result.Errors.Select(e => e.Description))}");
        }
        else
        {
            logger.LogInformation("Admin exists: {UserName}", admin.Email);
        }
    }
}