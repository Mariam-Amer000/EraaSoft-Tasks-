using Stripe.Checkout;
using System.Security.Claims;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CheckoutsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<CheckoutsController> _logger;

    public CheckoutsController(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Book> bookRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        ApplicationDbContext applicationDbContext,
        ILogger<CheckoutsController> logger)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Success(int orderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

        try
        {
            // 1. Update order status
            var order = _orderRepository.GetOne(
                e => e.Id == orderId && e.ApplicationUserId == user.Id);

            if (order is null || order.OrderStatus >= OrderStatus.Processing)
                return NotFound();

            var service = new SessionService();
            var session = service.Get(order.SessionId);

            order.PaymentDate = DateTime.Now;
            order.PaymentStatus = PaymentStatus.Succussed;
            order.OrderStatus = OrderStatus.Processing;
            order.TransactionId = session.PaymentIntentId;

            await _orderRepository.CommitAsync();

            // 2. Move to order items
            var carts = _cartRepository.Get(
                e => e.ApplicationUserId == user.Id,
                includes: [e => e.Book]);

            foreach (var item in carts)
            {
                await _orderItemRepository.CreateAsync(new()
                {
                    OrderId = orderId,
                    Quantity = item.Quantity,
                    Price = item.TotalPrice,
                    BookId = item.BookId
                });
            }

            await _orderItemRepository.CommitAsync();

            // 3. Decrease book stock
            foreach (var item in carts)
                item.Book.Stock -= item.Quantity;

            await _bookRepository.CommitAsync();

            // 4. Remove cart from database
            foreach (var item in carts)
                _cartRepository.Delete(item);

            await _cartRepository.CommitAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await transaction.RollbackAsync();
        }

        return Ok();
    }
}
