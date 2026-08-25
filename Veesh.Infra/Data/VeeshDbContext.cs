using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Veesh.Domain.Entities;
using Veesh.Infra.Configurations;

namespace Veesh.Infra.Data;

public class VeeshDbContext(DbContextOptions<VeeshDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<WishList> WishLists => Set<WishList>();
    public DbSet<Wish> Wishes => Set<Wish>();
    public DbSet<WishStoreUrl> WishStoreUrls => Set<WishStoreUrl>();

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