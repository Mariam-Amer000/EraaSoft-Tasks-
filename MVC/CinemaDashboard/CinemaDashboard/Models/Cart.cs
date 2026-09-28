namespace CinemaDashboard.Models;

public class Cart
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    public string SeatNumber { get; set; } = string.Empty; // مثل "A5"
    public DateTime Showtime { get; set; } // موعد العرض المختار

    public int Count { get; set; } = 1;
    public decimal CurrentPrice { get; set; }

    public string? PromotionCode { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
}