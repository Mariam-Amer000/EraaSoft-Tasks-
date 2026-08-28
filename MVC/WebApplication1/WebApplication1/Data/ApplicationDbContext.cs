namespace WebApplication1.Data;
public class ApplicationDbContext : DbContext
{

   public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=WebApllication1;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");
    }

}
