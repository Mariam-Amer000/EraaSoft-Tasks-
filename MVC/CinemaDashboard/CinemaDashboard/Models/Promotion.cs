namespace CinemaDashboard.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal Discount { get; set; }
    public bool Status { get; set; } = true;
    public DateTime ValidFrom { get; set; } = DateTime.Now;
    public DateTime ValidTo { get; set; }
    public int MaxUsage { get; set; } = 1;
    public int? MovieId { get; set; }
    public Movie? Movie { get; set; }
}