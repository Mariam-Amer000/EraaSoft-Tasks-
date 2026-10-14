namespace BookStore.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthYear { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}
