
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin;

[Route("api/[area]/[controller]")]
[ApiController]
[Area("Admin")]
public class BooksController : ControllerBase
{
    public IRepository<Book> _bookRepository;

    public BooksController(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }
    [HttpGet]
    [Route(nameof(Get))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE},{RoleConstants.CUSTOMER}")]
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
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE},{RoleConstants.CUSTOMER}")]
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
    public async Task<IActionResult> Create(Book book,CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bookRepository.CreateAsync(book, CT);
        if (result)
            await _bookRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Create Book Successfully"
        });
    }
    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
    public async Task<IActionResult> Update(int id, Book book,CancellationToken CT)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var bookInDb = _bookRepository.Get(e => e.Id == id).FirstOrDefault();
        if(bookInDb is null)
            return NotFound();

        bookInDb.Name = book.Name;
        bookInDb.Author = book.Author;
        bookInDb.Price = book.Price;
        bookInDb.CategoryId = book.CategoryId;
        bookInDb.Description = book.Description;

        await _bookRepository.CommitAsync(CT);
        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Update Book Successfully"
        });
    }
    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
    public async Task<IActionResult> Delete(int id,CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var bookInDb = _bookRepository.Get(e => e.Id == id).FirstOrDefault();
        if (bookInDb is null)
            return NotFound();

        _bookRepository.Delete(bookInDb);
        await _bookRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Delete Book Successfully"
        });
    }


}
