namespace CinemaDashboard.Models;

public class MovieActor
{
    public int Id { get; set; }
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int ActorId { get; set; }
    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;

}
