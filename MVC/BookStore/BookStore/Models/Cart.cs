namespace BookStore.Models;

public class Cart
{
    public  int Id { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public required string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

}
