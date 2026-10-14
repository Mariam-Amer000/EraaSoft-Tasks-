namespace BookStore.DTOs.Responses;

public class BookWithFilterResponse
{
    public IEnumerable<Book> Books { get; set; } = new List<Book>();
    public string Name { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }

}
