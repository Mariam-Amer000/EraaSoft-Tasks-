using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.DataAccess.EntityTypeConfiguration;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.HasOne(w => w.ApplicationUser)
            .WithMany()
            .HasForeignKey(w => w.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Book)
            .WithMany()
            .HasForeignKey(w => w.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}