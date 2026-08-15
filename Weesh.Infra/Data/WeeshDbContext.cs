using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Weesh.Domain.Entities;
using Weesh.Infra.Configurations;

namespace Weesh.Infra.Data;

public class WeeshDbContext(DbContextOptions<WeeshDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ApplicationUserConfig());
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // TODO: Set UpdatedAt and CreatedAt automatically 
        return base.SaveChangesAsync(cancellationToken);
    }

}