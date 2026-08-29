namespace WebApplication1.ModelView;

public class DoctorWithFilter
{
    public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();
    public string Name { get; set; } = string.Empty;
}
