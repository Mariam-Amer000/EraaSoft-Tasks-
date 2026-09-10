
namespace CinemaDashboard.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [LetteroOnly(5,30)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
