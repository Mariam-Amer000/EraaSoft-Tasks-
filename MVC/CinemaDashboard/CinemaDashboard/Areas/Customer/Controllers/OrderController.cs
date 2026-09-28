using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(RoleConstants.CUSTOMER)]
[Authorize]
public class OrderController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;

    public OrderController(
        UserManager<ApplicationUser> userManager,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<IActionResult> Index(int? query, int page = 1, int size = 4)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        // جلب طلبات المستخدم الحالي فقط لحماية الخصوصية
        var orders = _orderRepository.Get(e => e.ApplicationUserId == user.Id);

        if (query is not null)
            orders = orders.Where(e => e.Id == query);

        var totalCount = orders.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)size);

        var pagedOrders = orders
            .OrderByDescending(e => e.OrderDate)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return View(new OrderWithFilterVM
        {
            Orders = pagedOrders,
            Query = query,
            TotalPages = totalPages,
            CurrentPage = page,
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var order = _orderRepository.GetOne(e => e.Id == id && e.ApplicationUserId == user.Id);
        if (order is null) return NotFound();

        var orderItems = _orderItemRepository.Get(
            e => e.OrderId == id,
            includes: [e => e.Movie, e => e.Cinema, e => e.Order]
        ).ToList();

        return View(orderItems);
    }

    public async Task<IActionResult> Refund(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var order = _orderRepository.GetOne(e => e.Id == id && e.ApplicationUserId == user.Id);
        if (order is null) return NotFound();

        // منع الإلغاء إذا كان الطلب ملغى بالفعل
        if (order.OrderStatus == OrderStatus.Cancelled || string.IsNullOrEmpty(order.TransactionId))
            return BadRequest("لا يمكن إلغاء هذا الطلب.");

        var options = new RefundCreateOptions
        {
            Reason = RefundReasons.RequestedByCustomer,
            Amount = (long)(order.TotalPrice * 100), // ضرب المبلغ في 100 لتحويله إلى Cents/Piastres لـ Stripe
            PaymentIntent = order.TransactionId,
        };

        var service = new RefundService();
        var refund = await service.CreateAsync(options);

        // تحديث حالة الطلب والدفع في قاعدة البيانات بعد نجاح العملية
        order.OrderStatus = OrderStatus.Cancelled;
        order.PaymentStatus = PaymentStatus.Refunded;

        await _orderRepository.CommitAsync();

        TempData["Success"] = "تم إلغاء الحجز واسترداد المبلغ بنجاح.";
        return RedirectToAction(nameof(Index));
    }
}