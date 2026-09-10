namespace CinemaDashboard.Models;

public class Cinema
{
    public int Id { get; set; }
    [Required]
    [LetteroOnly(3,20)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Img { get; set; }
}
