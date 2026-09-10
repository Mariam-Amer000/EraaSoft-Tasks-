namespace CinemaDashboard.Models;

public class Movie
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MainImg { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public bool Status { get; set; }
    [Required]
    public DateTime DateTime { get; set; }
    [Required]
    public int CinemaId { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public Cinema Cinema { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
