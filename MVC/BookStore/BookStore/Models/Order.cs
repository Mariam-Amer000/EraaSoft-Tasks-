namespace BookStore.Models;


public class Order : Audit
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Stripe;
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? SessionId { get; set; }
    public DateTime? PaymentDate { get; set; }
}
