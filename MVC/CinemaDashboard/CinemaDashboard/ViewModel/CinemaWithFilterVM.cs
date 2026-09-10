namespace CinemaDashboard.ViewModel;

public class CinemaWithFilterVM
{
    public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
    public string Name { get; set; } = string.Empty;
    public double TotalPages {  get; set; }
}
