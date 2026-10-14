
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controller;

[Route("api/[area]/[controller]")]
[ApiController]
[Area(nameof(Customer))]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Promotion> _promotionRepository;

    public CartsController(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Book> bookRepository,
        IRepository<Promotion> promotionRepository)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _promotionRepository = promotionRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(
        int bookId, int quantity, CancellationToken ct = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var book = _bookRepository.GetOne(e => e.Id == bookId);
        if (book is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.BookId == bookId &&
                 e.ApplicationUserId == user.Id);

        if (cartInDB is not null)
        {
            cartInDB.Quantity += quantity;
            cartInDB.TotalPrice = book.Price * cartInDB.Quantity;
        }
        else
        {
            await _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = user.Id,
                Quantity = quantity,
                BookId = bookId,
                TotalPrice = book.Price * quantity
            }, ct);
        }

        await _cartRepository.CommitAsync(ct);

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Add Book Successfully"
        });
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? promo = null)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var carts = _cartRepository.Get(
            e => e.ApplicationUserId == user.Id,
            includes: [e => e.Book]).ToList();

        if (promo is not null)
        {
            var promotion = _promotionRepository.GetOne(e =>
                e.Code == promo &&
                e.IsActive &&
                e.StartDate <= DateTime.Now &&
                e.EndDate >= DateTime.Now);

            if (promotion is null)
            {
                return Ok(new ErrorResponse
                {
                    ErrorNotification =
                        "Invalid promo code or expired"
                });
            }

            var subtotal = carts.Sum(e => e.TotalPrice);

            var discountAmount =
                subtotal * (promotion.DiscountPercentage / 100m);

            var totalAfterDiscount = subtotal - discountAmount;

            return Ok(new
            {
                Carts = carts,
                Subtotal = subtotal,
                DiscountPercentage = promotion.DiscountPercentage,
                DiscountAmount = discountAmount,
                TotalPrice = totalAfterDiscount,
                PromoCode = promotion.Code
            });
        }

        return Ok(carts);
    }

    [HttpGet("IncrementQuantity/{id}")]
    public async Task<IActionResult> IncrementQuantity(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id,
            includes: [e => e.Book]);

        if (cartInDB is null) return NotFound();

        cartInDB.Quantity += 1;
        cartInDB.TotalPrice =
            cartInDB.Book.Price * cartInDB.Quantity;

        await _cartRepository.CommitAsync();

        return Ok();
    }

    [HttpGet("DecrementQuantity/{id}")]
    public async Task<IActionResult> DecrementQuantity(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id,
            includes: [e => e.Book]);

        if (cartInDB is null) return NotFound();

        if (cartInDB.Quantity > 1)
        {
            cartInDB.Quantity -= 1;
            cartInDB.TotalPrice =
                cartInDB.Book.Price * cartInDB.Quantity;

            await _cartRepository.CommitAsync();
        }

        return Ok();
    }

    [HttpDelete("DeleteBook/{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id);

        if (cartInDB is null) return NotFound();

        _cartRepository.Delete(cartInDB);
        await _cartRepository.CommitAsync();

        return Ok();
    }
}