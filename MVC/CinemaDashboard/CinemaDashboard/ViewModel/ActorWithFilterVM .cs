namespace CinemaDashboard.ViewModel;

public class ActorWithFilterVM
{
    public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
    public string Name { get; set; } = string.Empty;
    public double TotalPages {  get; set; }
}
