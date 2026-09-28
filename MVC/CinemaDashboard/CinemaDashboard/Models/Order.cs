public class Order
{
    public int Id { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? PaymentDate { get; set; }

    public decimal TotalPrice { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // معرّفات Stripe
    public string? SessionId { get; set; }
    public string? TransactionId { get; set; }
}