namespace BookStore.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; 
    public decimal DiscountPercentage { get; set; } 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}