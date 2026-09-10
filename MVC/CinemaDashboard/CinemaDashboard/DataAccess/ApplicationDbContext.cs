namespace CinemaDashboard.DataAccess;

public class ApplicationDbContext:DbContext
{
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Cinema> Cinemas { get; set; } 
    public DbSet<Movie> Movies { get; set; } 
    public DbSet<Actor> Actors { get; set; } 
    public DbSet<MovieActor> MovieActors { get; set; } 
    public DbSet<MovieSubImg> MovieSubImgs { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CinemaProject;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
