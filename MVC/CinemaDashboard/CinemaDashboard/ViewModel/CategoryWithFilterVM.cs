namespace CinemaDashboard.ViewModel;

public class CategoryWithFilterVM
{
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string Name { get; set; } = string.Empty;
    public double TotalPages {  get; set; }
}
