namespace BookStore.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class AuthorsController : ControllerBase
    {
        private readonly IRepository<Author> _authorRepository;

        public AuthorsController(IRepository<Author> authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        [Route(nameof(Get))]
        [AllowAnonymous]
        public IActionResult Get(string? name)
        {
            var authors = _authorRepository.Get();

            if (name is not null)
                authors = authors.Where(a => a.Name.Contains(name));

            return Ok(authors);
        }

        [HttpGet]
        [Route(nameof(GetOne))]
        [AllowAnonymous]
        public IActionResult GetOne(int id)
        {
            var author = _authorRepository.Get(e => e.Id == id).FirstOrDefault();

            if (author is null)
                return NotFound();

            return Ok(author);
        }

        [HttpPost]
        [Route(nameof(Create))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Create(Author author, CancellationToken CT)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authorRepository.CreateAsync(author, CT);

            if (!result)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not create Author."
                });

            if (await _authorRepository.CommitAsync(CT) == 0)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not save Author."
                });

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Create Author Successfully"
            });
        }

        [HttpPut]
        [Route(nameof(Update))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Update(int id, Author author, CancellationToken CT)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorInDb = _authorRepository.Get(e => e.Id == id).FirstOrDefault();

            if (authorInDb is null)
                return NotFound();

            authorInDb.Name = author.Name;
            authorInDb.BirthYear = author.BirthYear;

            await _authorRepository.CommitAsync(CT);

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Update Author Successfully"
            });
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Delete(int id, CancellationToken CT)
        {
            var authorInDb = _authorRepository.Get(e => e.Id == id).FirstOrDefault();

            if (authorInDb is null)
                return NotFound();

            _authorRepository.Delete(authorInDb);

            await _authorRepository.CommitAsync(CT);

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Delete Author Successfully"
            });
        }
    }
}