namespace BookStore.Models;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Image { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public int SalesNumber { get; set; } = 0;
    public decimal FinalPrice => Price - (Price * Discount / 100m);
    public int Stock { get; set; } = 0;
    public bool FlashSale { get; set; } = false;
    public decimal? FlashSaleFinalPrice { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;


}
