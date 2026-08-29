using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Veesh.Domain.Entities;

namespace Veesh.Infra.Configurations;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.HasIndex(e => e.PhoneNumber).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();

        builder
            .HasMany(u => u.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId);
    }
}
