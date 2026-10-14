namespace BookStore.Models;

public class Wishlist
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public required string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}
