using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veesh.Domain.Entities;

namespace Veesh.Infra.Configurations;

public class WishConfig : IEntityTypeConfiguration<Wish>
{
    public void Configure(EntityTypeBuilder<Wish> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.WishList)
            .WithMany(wl => wl.Wishes)
            .HasForeignKey(e => e.WishListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WishListId);
    }
}