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
    private readonly IRepository<Promotion> _promotionRepository;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<CheckoutsController> _logger;
    private readonly IConfiguration _configuration;

    public CheckoutsController(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Book> bookRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Promotion> promotionRepository,
        ApplicationDbContext applicationDbContext,
        ILogger<CheckoutsController> logger,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _promotionRepository = promotionRepository;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _configuration = configuration;
    }

    private decimal GetBookPrice(Book book)
    {
        if (book.FlashSale && book.FlashSaleFinalPrice.HasValue)
            return book.FlashSaleFinalPrice.Value;

        return book.FinalPrice;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckout(
        string? promo = null,
        CancellationToken ct = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var carts = _cartRepository.Get(
            e => e.ApplicationUserId == user.Id,
            includes: [e => e.Book]).ToList();

        if (!carts.Any())
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Your cart is empty."
            });
        }

        foreach (var item in carts)
        {
            if (item.Book.Stock < item.Quantity)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorNotification = $"Not enough stock for book: {item.Book.Name}"
                });
            }

            item.TotalPrice = GetBookPrice(item.Book) * item.Quantity;
        }

        var subtotal = carts.Sum(e => e.TotalPrice);
        var discountPercentage = 0m;

        if (!string.IsNullOrWhiteSpace(promo))
        {
            var promotion = _promotionRepository.GetOne(e =>
                e.Code == promo &&
                e.IsActive &&
                e.StartDate <= DateTime.Now &&
                e.EndDate >= DateTime.Now);

            if (promotion is null)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorNotification = "Invalid promo code or expired."
                });
            }

            discountPercentage = promotion.DiscountPercentage;
        }

        var totalPrice = subtotal - subtotal * discountPercentage / 100m;

        if (totalPrice <= 0)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Order total must be greater than zero."
            });
        }

        var order = new Order
        {
            ApplicationUserId = user.Id,
            TotalPrice = totalPrice,
            OrderStatus = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.Stripe
        };

        var created = await _orderRepository.CreateAsync(order, ct);

        if (!created)
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not create the order."
            });
        }

        if (await _orderRepository.CommitAsync(ct) == 0)
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not save the order."
            });
        }

        var priceRatio = subtotal > 0 ? totalPrice / subtotal : 1m;

        foreach (var item in carts)
        {
            var createdItem = await _orderItemRepository.CreateAsync(new OrderItem
            {
                OrderId = order.Id,
                Quantity = item.Quantity,
                Price = item.TotalPrice * priceRatio,
                BookId = item.BookId
            }, ct);

            if (!createdItem)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not save the order items."
                });
        }

        if (await _orderItemRepository.CommitAsync(ct) == 0)
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not save the order items."
            });

        try
        {
            var currency = _configuration["Stripe:Currency"] ?? "usd";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = (long)Math.Round(
                                totalPrice * 100m,
                                MidpointRounding.AwayFromZero),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"BookStore Order #{order.Id}"
                            }
                        }
                    }
                },
                SuccessUrl =
                    $"{Request.Scheme}://{Request.Host}/api/Checkouts/{order.Id}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl =
                    $"{Request.Scheme}://{Request.Host}/api/Checkouts/Cancel/{order.Id}",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() },
                    { "UserId", user.Id }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            order.SessionId = session.Id;

            if (await _orderRepository.CommitAsync(ct) == 0)
                throw new InvalidOperationException("Could not save Stripe session ID.");

            return Ok(new
            {
                OrderId = order.Id,
                CheckoutUrl = session.Url,
                TotalPrice = order.TotalPrice
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not create Stripe checkout session.");

            order.OrderStatus = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Canceled;
            await _orderRepository.CommitAsync(ct);

            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not create the payment session."
            });
        }
    }

    [HttpGet("{orderId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Success(
        int orderId,
        string session_id,
        CancellationToken ct = default)
    {
        var order = _orderRepository.GetOne(e => e.Id == orderId);

        if (order is null)
            return NotFound();

        if (order.OrderStatus != OrderStatus.Pending ||
            string.IsNullOrWhiteSpace(order.SessionId) ||
            order.SessionId != session_id)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid or already processed order."
            });
        }

        try
        {
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus != "paid")
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorNotification = "Payment has not been completed."
                });
            }

            if (session.Metadata is null ||
                !session.Metadata.TryGetValue("OrderId", out var metadataOrderId) ||
                metadataOrderId != order.Id.ToString() ||
                !session.Metadata.TryGetValue("UserId", out var metadataUserId) ||
                metadataUserId != order.ApplicationUserId ||
                session.AmountTotal != (long)Math.Round(
                    order.TotalPrice * 100m,
                    MidpointRounding.AwayFromZero))
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorNotification = "Payment session does not match this order."
                });
            }

            await using var transaction =
                await _applicationDbContext.Database.BeginTransactionAsync(ct);

            var orderItems = _orderItemRepository.Get(
                e => e.OrderId == order.Id,
                includes: [e => e.Book]).ToList();

            if (!orderItems.Any())
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorNotification = "The order has no items."
                });
            }

            foreach (var item in orderItems)
            {
                if (item.Book.Stock < item.Quantity)
                {
                    _logger.LogError(
                        "Paid order {OrderId} has insufficient stock for book {BookId}.",
                        order.Id,
                        item.BookId);

                    return Conflict(new ErrorResponse
                    {
                        ErrorNotification = "Payment received, but stock must be reviewed by support."
                    });
                }
            }

            order.PaymentDate = DateTime.Now;
            order.PaymentStatus = PaymentStatus.Succussed;
            order.OrderStatus = OrderStatus.Processing;
            order.TransactionId = session.PaymentIntentId;

            foreach (var item in orderItems)
            {
                item.Book.Stock -= item.Quantity;
                item.Book.SalesNumber += item.Quantity;
            }

            var carts = _cartRepository.Get(
                e => e.ApplicationUserId == order.ApplicationUserId).ToList();

            foreach (var item in orderItems)
            {
                var cart = carts.FirstOrDefault(e =>
                    e.BookId == item.BookId &&
                    e.Quantity == item.Quantity);

                if (cart is not null)
                    _cartRepository.Delete(cart);
            }

            await _orderRepository.CommitAsync(ct);
            await _orderItemRepository.CommitAsync(ct);
            await _bookRepository.CommitAsync(ct);
            await _cartRepository.CommitAsync(ct);

            await transaction.CommitAsync(ct);

            return Ok(new SuccessResponse
            {
                SuccessNotification = "Payment completed and order created successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while completing order {OrderId}", orderId);

            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "An error occurred while completing your order."
            });
        }
    }

    [HttpGet("Cancel/{orderId}")]
    [AllowAnonymous]
    public IActionResult Cancel(int orderId)
    {
        var order = _orderRepository.GetOne(e => e.Id == orderId);

        if (order is null)
            return NotFound();

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Checkout was cancelled. Your cart has been kept; you can try again."
        });
    }
}