
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

    private decimal GetBookPrice(Book book)
    {
        if (book.FlashSale && book.FlashSaleFinalPrice.HasValue)
            return book.FlashSaleFinalPrice.Value;

        return book.FinalPrice;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(
        int bookId,
        int quantity,
        CancellationToken ct = default)
    {
        if (quantity <= 0)
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Quantity must be greater than zero."
            });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var book = _bookRepository.GetOne(e => e.Id == bookId);

        if (book is null)
            return NotFound(new ErrorResponse
            {
                ErrorNotification = "Book not found."
            });

        var cartInDB = _cartRepository.GetOne(
            e => e.BookId == bookId &&
                 e.ApplicationUserId == user.Id);

        var currentQuantity = cartInDB?.Quantity ?? 0;

        if (book.Stock < currentQuantity + quantity)
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "The requested quantity is not available in stock."
            });

        var bookPrice = GetBookPrice(book);

        if (cartInDB is not null)
        {
            cartInDB.Quantity += quantity;
            cartInDB.TotalPrice = bookPrice * cartInDB.Quantity;
        }
        else
        {
            var created = await _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = user.Id,
                Quantity = quantity,
                BookId = bookId,
                TotalPrice = bookPrice * quantity
            }, ct);

            if (!created)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not add book to cart."
                });
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

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var carts = _cartRepository.Get(
            e => e.ApplicationUserId == user.Id,
            includes: [e => e.Book]).ToList();

        var subtotal = carts.Sum(e => e.TotalPrice);
        var discountPercentage = 0m;
        var promoCode = (string?)null;

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
            promoCode = promotion.Code;
        }

        var discountAmount = subtotal * discountPercentage / 100m;
        var totalPrice = subtotal - discountAmount;

        return Ok(new
        {
            Carts = carts,
            Subtotal = subtotal,
            DiscountPercentage = discountPercentage,
            DiscountAmount = discountAmount,
            TotalPrice = totalPrice,
            PromoCode = promoCode
        });
    }

    [HttpGet("IncrementQuantity/{id}")]
    public async Task<IActionResult> IncrementQuantity(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id,
            includes: [e => e.Book]);

        if (cartInDB is null)
            return NotFound();

        if (cartInDB.Quantity >= cartInDB.Book.Stock)
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "The requested quantity is not available in stock."
            });

        cartInDB.Quantity++;
        cartInDB.TotalPrice =
            GetBookPrice(cartInDB.Book) * cartInDB.Quantity;

        await _cartRepository.CommitAsync();

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Quantity increased successfully."
        });
    }

    [HttpGet("DecrementQuantity/{id}")]
    public async Task<IActionResult> DecrementQuantity(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id,
            includes: [e => e.Book]);

        if (cartInDB is null)
            return NotFound();

        if (cartInDB.Quantity > 1)
        {
            cartInDB.Quantity--;
            cartInDB.TotalPrice =
                GetBookPrice(cartInDB.Book) * cartInDB.Quantity;

            await _cartRepository.CommitAsync();
        }

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Quantity decreased successfully."
        });
    }

    [HttpDelete("DeleteBook/{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var cartInDB = _cartRepository.GetOne(
            e => e.ApplicationUserId == user.Id && e.Id == id);

        if (cartInDB is null)
            return NotFound();

        _cartRepository.Delete(cartInDB);

        await _cartRepository.CommitAsync();

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Book removed from cart successfully."
        });
    }
}
