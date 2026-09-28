namespace CinemaDashboard.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

        [Required]
        public string SeatNumber { get; set; } = string.Empty;

        public DateTime Showtime { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }
    }
}
