using WebApplication1.Data;

namespace WebApplication1.Areas.Patient.Controllers;

[Area(AreaConstant.PATEINT_AREA)]
public class HomeController : Controller
{
    public readonly ApplicationDbContext _db = new();
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult BookAppointment()
    {
        IQueryable<Doctor> doctors = _db.Doctors
            .Include(d => d.Specialization);
        return View(doctors);
    }
    public IActionResult CompleteAppointment()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
