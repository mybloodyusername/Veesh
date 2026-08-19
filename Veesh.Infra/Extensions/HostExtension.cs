using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Extensions;

public static class HostExtension
{
    extension(IHost host)
    {
        public async Task InitializeVeeshDbAsync()
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<VeeshDbContext>>();
            var context = services.GetRequiredService<VeeshDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await context.Database.MigrateAsync();
            await SeedDatabase.Initialize(logger, services, userManager, roleManager);
        }
    }
}