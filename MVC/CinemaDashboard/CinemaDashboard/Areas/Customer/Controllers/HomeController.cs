namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(RoleConstants.CUSTOMER)]
public class HomeController : Controller
{
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Cinema> _cinemaRepository;

    public HomeController(IRepository<Movie> movieRepository, IRepository<Cinema> cinemaRepository)
    {
        _movieRepository = movieRepository;
        _cinemaRepository = cinemaRepository;
    }

    public IActionResult Index()
    {
        var movies = _movieRepository.Get(includes: [m => m.Category]).ToList();
        return View(movies);
    }

    public IActionResult Details(int id)
    {
        var movie = _movieRepository.GetOne(m => m.Id == id, includes: [m => m.Category]);
        if (movie is null) return NotFound();

        ViewBag.Cinemas = _cinemaRepository.Get().ToList();

        return View(movie);
    }
}