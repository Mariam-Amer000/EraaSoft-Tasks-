namespace CinemaDashboard.Models;

public class MovieSubImg
{
    public int Id { get; set; }
    [Required]
    public string? SubImg { get; set; }
    [Required]
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}
