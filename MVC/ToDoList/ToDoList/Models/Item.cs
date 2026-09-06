namespace ToDoList.Models;

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public string? FilePath { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
