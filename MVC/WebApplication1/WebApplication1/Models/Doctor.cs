namespace WebApplication1.Models;

public class Doctor
{
    public int Id{ get; set; }
    public string Name { get; set; } = string.Empty;
    public string MainImg { get; set; }=string.Empty;
    public int specializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;

}
