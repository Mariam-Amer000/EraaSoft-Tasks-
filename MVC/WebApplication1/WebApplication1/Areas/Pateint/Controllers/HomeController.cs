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
    [HttpGet]
    public IActionResult CompleteAppointment(int Id)
    {
        return View(Id);
    }


    [HttpPost]
    public IActionResult CompleteAppointment(int DoctorId ,string PatientName, DateOnly AppointmentDate, TimeOnly AppointmentTime)
    {
        WebApplication1.Models.Patient? patient = _db.Patients
         .SingleOrDefault(p => p.Name == PatientName);

        if (patient is null)
        {
            patient = new WebApplication1.Models.Patient()
            {
                Name = PatientName
            };

            _db.Patients.Add(patient);
            _db.SaveChanges();
        }

        _db.Enrollments.Add(new Enrollment()
        {
            DoctorId = DoctorId,
            PatientId = patient.Id,
            AppointmentDate = AppointmentDate,
            AppointmentTime = AppointmentTime
        }); 

        _db.SaveChanges();
        return RedirectToAction(nameof(BookAppointment));
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
