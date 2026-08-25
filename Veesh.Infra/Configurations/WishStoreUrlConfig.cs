using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veesh.Domain.Entities;

namespace Veesh.Infra.Configurations;

public class WishStoreUrlConfig : IEntityTypeConfiguration<WishStoreUrl>
{
    public void Configure(EntityTypeBuilder<WishStoreUrl> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Price).HasPrecision(18, 2);

        builder.HasOne(e => e.Wish)
            .WithMany(w => w.Stores)
            .HasForeignKey(e => e.WishId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WishId);
    }
}