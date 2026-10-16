namespace BookStore.Areas.Customer.Controller;

[Route("api/[area]/[controller]")]
[ApiController]
[Area(nameof(Customer))]
public class HomeController : ControllerBase
{
    public readonly IRepository<Author> _authorRepository;
    public readonly IRepository<Book> _bookRepository;

    public HomeController(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    [HttpGet]
    [Route(nameof(Search))]
    [AllowAnonymous]
    public IActionResult Search(string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "Search string is required."
            });
        }

        var books = _bookRepository.Get(b =>
            b.Name.Contains(searchString) ||
            b.Author.Name.Contains(searchString));

        return Ok(books);
    }

    [HttpGet]
    [Route(nameof(BestSeller))]
    [AllowAnonymous]
    public IActionResult BestSeller()
    {
        var books = _bookRepository.Get()
            .OrderByDescending(b => b.SalesNumber)
            .Take(8);

        return Ok(books);
    }

    [HttpGet]
    [Route(nameof(RecommendedBooks))]
    [AllowAnonymous]
    public IActionResult RecommendedBooks()
    {
        var books = _bookRepository.Get()
            .OrderByDescending(b => b.SalesNumber)
            .Take(8);

        return Ok(books);
    }

    [HttpGet]
    [Route(nameof(FlashSale))]
    [AllowAnonymous]
    public IActionResult FlashSale()
    {
        var books = _bookRepository.Get(b =>
            b.FlashSale && b.Stock > 0);

        return Ok(books);
    }
}