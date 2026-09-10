namespace CinemaDashboard.Areas.Admin.Controllers;


[Area("Admin")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db=new ApplicationDbContext();
    public IActionResult Index()
    {
        var statsModel = new DashboardVM
        {
            CategoriesCount = _db.Categories.Count(),
            CinemasCount = _db.Cinemas.Count(),
            ActorsCount = _db.Actors.Count(),
            MoviesCount = _db.Movies.Count()
        };

        return View(statsModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
