namespace WebApplication1.Models;

public class Specialization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Doctor> Doctors { get; } = new List<Doctor>();
}
