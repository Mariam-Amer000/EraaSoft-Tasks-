using CinemaDashboard.Validations;
using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;
public class Actor
{
    public int Id { get; set; }
    [Required]
    [LetteroOnly(3,20)]
    public string Name { get; set; } = string.Empty;
    public string? Img { get; set; }
}
