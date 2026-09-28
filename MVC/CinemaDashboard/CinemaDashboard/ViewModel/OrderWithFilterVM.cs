namespace CinemaDashboard.ViewModel;

public class OrderWithFilterVM
{
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    public int? Query { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
