namespace CinemaDashboard.ViewModel;

public class MovieWithFilterVM
{
    public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
    public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? MainImg { get; set; }
    public decimal? Maxprice { get; set; }
    public decimal? Minprice { get; set; }
    public bool? Status { get; set; }
    public DateTime? DateTime { get; set; }
    public int? CinemaId { get; set; }
    public int? CategoryId { get; set; }
    public double TotalPages {  get; set; }
}
