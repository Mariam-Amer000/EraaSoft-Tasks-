using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(RoleConstants.CUSTOMER)]
[Authorize]
public class CheckoutController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Movie> movieRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        ApplicationDbContext applicationDbContext,
        ILogger<CheckoutController> logger)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _movieRepository = movieRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<IActionResult> Success(int orderId)
    {
        if (TempData["fromPaymentAction"] is null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

        try
        {
            // 1. تحديث حالة الطلب
            var order = _orderRepository.GetOne(e => e.Id == orderId && e.ApplicationUserId == user.Id);
            if (order is null || order.OrderStatus >= OrderStatus.InProcessing) return NotFound();

            var service = new SessionService();
            var session = service.Get(order.SessionId);

            order.PaymentDate = DateTime.Now;
            order.PaymentStatus = PaymentStatus.Succussed;
            order.OrderStatus = OrderStatus.InProcessing;
            order.TransactionId = session.PaymentIntentId;

            await _orderRepository.CommitAsync();

            // 2. تحويل عناصر السلة إلى تذاكر/عناصر طلب (OrderItem)
            var carts = _cartRepository.Get(
                e => e.ApplicationUserId == user.Id,
                includes: [e => e.Movie, e => e.Cinema]
            ).ToList();

            foreach (var item in carts)
            {
                await _orderItemRepository.CreateAsync(new OrderItem
                {
                    OrderId = orderId,
                    Count = item.Count,
                    Price = item.CurrentPrice,
                    MovieId = item.MovieId,
                    CinemaId = item.CinemaId,
                    SeatNumber = item.SeatNumber,
                    Showtime = item.Showtime
                });
            }
            await _orderItemRepository.CommitAsync();

            // 3. حذف عناصر السلة بعد نجاح الحجز
            foreach (var item in carts)
            {
                _cartRepository.Delete(item);
            }
            await _cartRepository.CommitAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تأكيد عملية الدفع للطلب رقم {OrderId}", orderId);
            await transaction.RollbackAsync();
            return BadRequest();
        }

        return View(orderId);
    }

    public IActionResult Cancel()
    {
        if (TempData["fromPaymentAction"] is null) return NotFound();

        // يمكن إضافة رسالة تنبيه للمستخدم أو إعادة توجيهه للسلة
        return View();
    }
}