namespace BookStore.DTOs.Responses
{
    public class ReviewWithFilterResponse
    {
        public IEnumerable<UserBookReview> Reviews { get; set; }
       = new List<UserBookReview>();

        public string Query { get; set; } = string.Empty;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
