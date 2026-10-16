
using BookStore.Helper;

namespace BookStore.Areas.Admin;

[Route("api/[area]/[controller]")]
[ApiController]
[Area("Admin")]
public class BooksController : ControllerBase
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IFileUpload _fileUpload;

    public BooksController(
        IRepository<Book> bookRepository,
        IFileUpload fileUpload)
    {
        _bookRepository = bookRepository;
        _fileUpload = fileUpload;
    }

    [HttpGet]
    [Route(nameof(Get))]
    [AllowAnonymous]
    public IActionResult Get(string? name, int page = 1, int size = 5)
    {
        var books = _bookRepository.Get();

        if (name is not null)
            books = books.Where(b => b.Name.Contains(name));

        int totalPages = (int)Math.Ceiling((double)books.Count() / size);

        books = books.Skip((page - 1) * size).Take(size);

        return Ok(new BookWithFilterResponse
        {
            Books = books,
            Name = name ?? string.Empty,
            TotalPages = totalPages,
            CurrentPage = page
        });
    }

    [HttpGet]
    [Route(nameof(GetOne))]
    [AllowAnonymous]
    public IActionResult GetOne(int id)
    {
        var book = _bookRepository.Get(e => e.Id == id).FirstOrDefault();

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    [Route(nameof(Create))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
    public async Task<IActionResult> Create(Book book, IFormFile Img, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (Img is not null && Img.Length > 0)
        {
            var fileName = _fileUpload.GenerateFileName(Img.FileName);

            var path = _fileUpload.GenerateFullPath(
                FileType.Img, "Books", fileName);

            if (path is not null)
            {
                var directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                _fileUpload.UploadFileLocally(path, Img);

                book.Image = fileName;
            }
        }

        var result = await _bookRepository.CreateAsync(book, CT);

        if (!result)
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not create Book."
            });

        if (await _bookRepository.CommitAsync(CT) == 0)
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not save Book."
            });

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Create Book Successfully"
        });
    }

    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
    public async Task<IActionResult> Update(int id, Book book, IFormFile? Img, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var bookInDb = _bookRepository.Get(e => e.Id == id).FirstOrDefault();

        if (bookInDb is null)
            return NotFound();

        bookInDb.Name = book.Name;
        bookInDb.AuthorId = book.AuthorId;
        bookInDb.Price = book.Price;
        bookInDb.CategoryId = book.CategoryId;
        bookInDb.Description = book.Description;
        bookInDb.Year = book.Year;
        bookInDb.Discount = book.Discount;
        bookInDb.Stock = book.Stock;
        bookInDb.FlashSale = book.FlashSale;
        bookInDb.FlashSaleFinalPrice = book.FlashSaleFinalPrice;

        if (Img is not null && Img.Length > 0)
        {
            var fileName = _fileUpload.GenerateFileName(Img.FileName);

            var path = _fileUpload.GenerateFullPath(
                FileType.Img, "Books", fileName);

            if (path is not null)
            {
                var directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                _fileUpload.UploadFileLocally(path, Img);

                if (!string.IsNullOrEmpty(bookInDb.Image))
                {
                    var oldImagePath = _fileUpload.GenerateFullPath(
                        FileType.Img, "Books", bookInDb.Image);

                    if (oldImagePath is not null)
                        _fileUpload.DeleteFileLocally(oldImagePath);
                }

                bookInDb.Image = fileName;
            }
        }

        await _bookRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Update Book Successfully"
        });
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
    public async Task<IActionResult> Delete(int id, CancellationToken CT)
    {
        var bookInDb = _bookRepository.Get(e => e.Id == id).FirstOrDefault();

        if (bookInDb is null)
            return NotFound();

        if (!string.IsNullOrEmpty(bookInDb.Image))
        {
            var path = _fileUpload.GenerateFullPath(
                FileType.Img, "Books", bookInDb.Image);

            if (path is not null)
                _fileUpload.DeleteFileLocally(path);
        }

        _bookRepository.Delete(bookInDb);

        await _bookRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Delete Book Successfully"
        });
    }
}
