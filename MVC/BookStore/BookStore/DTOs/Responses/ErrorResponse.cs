namespace BookStore.DTOs.Responses
{
    public class ErrorResponse
    {
        public Guid Guid { get; set; } = new Guid();
        public string? ErrorNotification { get; set; }
        public List<object>? Data { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
