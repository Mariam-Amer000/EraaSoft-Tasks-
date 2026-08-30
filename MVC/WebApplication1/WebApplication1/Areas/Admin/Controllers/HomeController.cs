
using WebApplication1.Data;

namespace WebApplication1.Areas.Admin.Controllers;


[Area(AreaConstant.ADMIN_AREA)]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db = new();
    public IActionResult ReservationsManagement()
    {
        IQueryable<Enrollment> enrollments = _db.Enrollments
         .Include(e => e.Doctor)
         .Include(e => e.Patient);

        return View(enrollments);
    }
}
