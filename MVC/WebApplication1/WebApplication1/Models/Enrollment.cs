namespace WebApplication1.Models;

public class Enrollment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public int PatientId { get; set; }

    public DateOnly AppointmentDate { get; set; }
    public TimeOnly AppointmentTime { get; set; }

    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}