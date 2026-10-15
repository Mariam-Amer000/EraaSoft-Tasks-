
using System.Security.Claims;

namespace BookStore.Areas.Customer.Controller;

[Route("api/[area]/[controller]")]
[ApiController]
[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize]
public class WishlistsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Wishlist> _wishlistRepository;
    private readonly IRepository<Book> _bookRepository;

    public WishlistsController(
        UserManager<ApplicationUser> userManager,
        IRepository<Wishlist> wishlistRepository,
        IRepository<Book> bookRepository)
    {
        _userManager = userManager;
        _wishlistRepository = wishlistRepository;
        _bookRepository = bookRepository;
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

        var wishlists = _wishlistRepository.Get(
            e => e.ApplicationUserId == user.Id,
            includes: [e => e.Book]);

        return Ok(wishlists);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(int bookId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var book = _bookRepository.GetOne(e => e.Id == bookId);

        if (book is null)
            return NotFound();

        var wishlistInDB = _wishlistRepository.GetOne(
            e => e.BookId == bookId &&
                 e.ApplicationUserId == user.Id);

        if (wishlistInDB is not null)
        {
            return Ok(new SuccessResponse
            {
                SuccessNotification = "Book already exists in wishlist"
            });
        }

        await _wishlistRepository.CreateAsync(new Wishlist
        {
            ApplicationUserId = user.Id,
            BookId = bookId
        });

        await _wishlistRepository.CommitAsync();

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Add Book To Wishlist Successfully"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var wishlistInDB = _wishlistRepository.GetOne(
            e => e.Id == id &&
                 e.ApplicationUserId == user.Id);

        if (wishlistInDB is null)
            return NotFound();

        _wishlistRepository.Delete(wishlistInDB);

        await _wishlistRepository.CommitAsync();

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Delete Book From Wishlist Successfully"
        });
    }
}
