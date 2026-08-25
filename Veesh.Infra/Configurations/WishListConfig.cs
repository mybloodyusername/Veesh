using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veesh.Domain.Entities;

namespace Veesh.Infra.Configurations;

public class WishListConfig : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Visibility).HasConversion<string>();

        builder.HasOne(e => e.Owner)
            .WithMany(au => au.WishLists)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.OwnerId);
    }
}