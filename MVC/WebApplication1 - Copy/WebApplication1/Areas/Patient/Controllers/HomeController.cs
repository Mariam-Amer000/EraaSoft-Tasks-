namespace WebApplication1.Areas.Patient.Controllers;

[Area(AreaConstant.PATEINT_AREA)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult BookAppointment()
    {
        return View();
    }
    public IActionResult CompleteAppointment()
    {
        return View();
    }
    public IActionResult ReservationsManagement()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
