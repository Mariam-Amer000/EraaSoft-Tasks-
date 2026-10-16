using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.DataAccess.EntityTypeConfiguration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Price)
            .HasPrecision(18, 2);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.Image)
            .IsRequired(true);

        builder.Property(b => b.Stock)
            .HasDefaultValue(0);

        builder.Property(b => b.SalesNumber)
            .HasDefaultValue(0);

        builder.Property(b => b.FlashSale)
            .HasDefaultValue(false);

        builder.Property(b => b.FlashSaleFinalPrice)
            .HasPrecision(18, 2);


        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_Book_Discount",
                "[Discount] >= 0 AND [Discount] <= 100"));

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
