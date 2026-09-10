namespace CinemaDashboard.ViewModel;

public class MovieWithDetalies
{
    public Movie? Movie { get; set; } = null!;
    public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
}
