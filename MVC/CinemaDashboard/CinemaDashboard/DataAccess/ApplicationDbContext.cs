using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CinemaDashboard.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Cinema> Cinemas { get; set; } 
    public DbSet<Movie> Movies { get; set; } 
    public DbSet<Actor> Actors { get; set; } 
    public DbSet<MovieActor> MovieActors { get; set; } 
    public DbSet<MovieSubImg> MovieSubImgs { get; set; }
    public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CinemaProject;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cart>()
        .HasOne(c => c.Movie)
        .WithMany()
        .HasForeignKey(c => c.MovieId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.Cinema)
            .WithMany()
            .HasForeignKey(c => c.CinemaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.ApplicationUser)
            .WithMany()
            .HasForeignKey(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Movie)
        .WithMany()
        .HasForeignKey(oi => oi.MovieId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Cinema)
            .WithMany()
            .HasForeignKey(oi => oi.CinemaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
