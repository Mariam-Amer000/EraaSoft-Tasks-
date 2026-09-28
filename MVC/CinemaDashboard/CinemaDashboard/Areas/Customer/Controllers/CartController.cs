using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace CinemaDashboard.Areas.Customer.Controllers;

[Area(RoleConstants.CUSTOMER)]
[Authorize(Roles = RoleConstants.CUSTOMER)]
public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Promotion> _promotionRepository;
    private readonly IRepository<Order> _orderRepository;

    public CartController(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Movie> movieRepository,
        IRepository<Promotion> promotionRepository,
        IRepository<Order> orderRepository)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _movieRepository = movieRepository;
        _promotionRepository = promotionRepository;
        _orderRepository = orderRepository;
    }

    // 1. إضافة تذكرة إلى السلة (AddToCart)
    public async Task<IActionResult> AddToCart(int movieId, int cinemaId, string seatNumber, DateTime showtime, int count = 1, CancellationToken ct = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var movie = _movieRepository.GetOne(e => e.Id == movieId);
        if (movie is null) return NotFound();

        // البحث عما إذا كان نفس المقعد والحجز موجودين مسبقاً بالسلة
        var cartInDB = _cartRepository.GetOne(e =>
            e.MovieId == movieId &&
            e.CinemaId == cinemaId &&
            e.SeatNumber == seatNumber &&
            e.ApplicationUserId == user.Id);

        if (cartInDB is not null)
        {
            cartInDB.Count += count;
        }
        else
        {
            await _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = user.Id,
                MovieId = movieId,
                CinemaId = cinemaId,
                SeatNumber = seatNumber,
                Showtime = showtime,
                Count = count,
                CurrentPrice = movie.Price // السعر الأساسي للفيلم
            }, ct);
        }

        await _cartRepository.CommitAsync(ct);

        TempData["Success"] = "تمت إضافة التذكرة إلى السلة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(string? promo = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        // جلب جميع التذاكر الخاصة بالمستخدم مع بيانات الفيلم والسينما
        var carts = _cartRepository.Get(
            e => e.ApplicationUserId == user.Id,
            includes: [e => e.Movie, e => e.Cinema]
        ).ToList();

        // تطبيق كود الخصم (Promotions)
        if (promo is not null)
        {
            var promotion = _promotionRepository.GetOne(e =>
                e.Code == promo &&
                e.Status &&
                e.ValidTo >= DateTime.Now &&
                e.MaxUsage >= 1);

            bool isValidPromo = false;

            if (promotion is not null)
            {
                foreach (var item in carts)
                {
                    // تطبيق الخصم على التذكرة المحددة في البروموكود أو كل التذاكر
                    if (item.MovieId == promotion.MovieId)
                    {
                        var priceAfterDiscount = item.Movie.Price - (item.Movie.Price * (promotion.Discount / 100m));
                        item.CurrentPrice = priceAfterDiscount;
                        _cartRepository.Update(item);
                        await _cartRepository.CommitAsync();
                        isValidPromo = true;
                        break;
                    }
                }
            }

            if (isValidPromo)
                TempData["Success"] = "تم تطبيق كود الخصم بنجاح";
            else
                TempData["Error"] = "كود الخصم غير صالح أو منتهي الصلاحية";
        }

        return View(carts);
    }

    public async Task<IActionResult> IncrementQuantity(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        cartInDB.Count += 1;
        await _cartRepository.CommitAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DecrementQuantity(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        if (cartInDB.Count > 1)
        {
            cartInDB.Count -= 1;
            await _cartRepository.CommitAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteProduct(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        _cartRepository.Delete(cartInDB);
        await _cartRepository.CommitAsync();

        return RedirectToAction(nameof(Index));
    }

    // 6. إنهاء عملية الدفع عبر Stripe Checkout (تطبيق نفس أسلوب الكورس)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(CancellationToken ct = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var carts = _cartRepository.Get(e => e.ApplicationUserId == user.Id,
            includes: [e => e.Movie, e => e.Cinema]);

        Order order = new()
        {
            ApplicationUserId = user.Id,
            OrderStatus = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            TotalPrice = carts.Sum(e => e.CurrentPrice * e.Count),
        };

        await _orderRepository.CreateAsync(order, ct);
        await _orderRepository.CommitAsync(ct);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success?orderId={order.Id}",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
        };

        foreach (var item in carts)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Movie?.Name ?? "تذكرة سينما",
                        Description = $"مقعد: {item.SeatNumber} | الموعد: {item.Showtime:yyyy-MM-dd hh:mm tt}",
                    },
                    UnitAmount = (long)(item.CurrentPrice * 100),
                },
                Quantity = item.Count,
            });
        }

        var service = new SessionService();
        var session = service.Create(options);

        order.SessionId = session.Id;
        await _orderRepository.CommitAsync(ct);

        TempData["fromPaymentAction"] = Guid.NewGuid();

        return Redirect(session.Url);
    }
}