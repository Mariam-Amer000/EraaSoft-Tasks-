namespace BookStore.DTOs.Responses
{
    public class SuccessResponse
    {
        public Guid Guid { get; set; } = new Guid();
        public string? SuccessNotification { get; set; }
        public List<object>? Data { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
