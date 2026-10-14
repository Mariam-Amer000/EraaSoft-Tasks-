namespace BookStore.Models;

public class UserBookReview
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Rate { get; set; }
    public string? Comment { get; set; }
    public string? Reply { get; set; }
}
