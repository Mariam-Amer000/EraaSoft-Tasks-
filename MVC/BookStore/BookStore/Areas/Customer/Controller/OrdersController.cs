
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controller;

[Route("api/[area]/[controller]")]
[ApiController]
[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;

    public OrdersController(
        UserManager<ApplicationUser> userManager,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var orders = _orderRepository.Get(
            e => e.ApplicationUserId == user.Id)
            .OrderByDescending(e => e.Id)
            .ToList();

        var result = orders.Select(order => new
        {
            Order = order,
            OrderItems = _orderItemRepository.Get(
                e => e.OrderId == order.Id,
                includes: [e => e.Book]).ToList()
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var order = _orderRepository.GetOne(
            e => e.Id == id &&
                 e.ApplicationUserId == user.Id);

        if (order is null)
            return NotFound();

        var orderItems = _orderItemRepository.Get(
            e => e.OrderId == order.Id,
            includes: [e => e.Book]).ToList();

        return Ok(new
        {
            Order = order,
            OrderItems = orderItems
        });
    }
}
