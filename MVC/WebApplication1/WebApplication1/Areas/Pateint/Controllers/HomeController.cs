using WebApplication1.Data;
using WebApplication1.ModelView;

namespace WebApplication1.Areas.Patient.Controllers;

[Area(AreaConstant.PATEINT_AREA)]
public class HomeController : Controller
{
    public readonly ApplicationDbContext _db = new();
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult BookAppointment(string?name)
    {
        IQueryable<Doctor> doctors = _db.Doctors
            .Include(d => d.Specialization);

        if (!string.IsNullOrEmpty(name))
        {
            doctors = doctors.Where(d => d.Name.Contains(name));
        }
        return View(new DoctorWithFilter
        {
            Doctors = doctors,
            Name = name ?? ""
        });
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
