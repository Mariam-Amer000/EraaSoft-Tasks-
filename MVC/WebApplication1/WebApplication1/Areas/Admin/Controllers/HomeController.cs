
namespace WebApplication1.Areas.Admin.Controllers;


[Area(AreaConstant.ADMIN_AREA)]
public class HomeController : Controller
{
    public IActionResult ReservationsManagement()
    {
        return View();
    }
}
