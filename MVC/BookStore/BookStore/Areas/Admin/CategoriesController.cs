namespace BookStore.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [Route(nameof(Get))]
        [AllowAnonymous]
        public IActionResult Get(string? name)
        {
            var categories = _categoryRepository.Get();

            if (name is not null)
                categories = categories.Where(c => c.Name.Contains(name));

            return Ok(categories);
        }

        [HttpGet]
        [Route(nameof(GetOne))]
        [AllowAnonymous]
        public IActionResult GetOne(int id)
        {
            var category = _categoryRepository.Get(e => e.Id == id).FirstOrDefault();

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        [Route(nameof(Create))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Create(Category category, CancellationToken CT)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryRepository.CreateAsync(category, CT);

            if (!result)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not create Category."
                });

            if (await _categoryRepository.CommitAsync(CT) == 0)
                return StatusCode(500, new ErrorResponse
                {
                    ErrorNotification = "Could not save Category."
                });

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Create Category Successfully"
            });
        }

        [HttpPut]
        [Route(nameof(Update))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Update(int id, Category category, CancellationToken CT)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryInDb = _categoryRepository.Get(e => e.Id == id).FirstOrDefault();

            if (categoryInDb is null)
                return NotFound();

            categoryInDb.Name = category.Name;
            categoryInDb.Description = category.Description;

            await _categoryRepository.CommitAsync(CT);

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Update Category Successfully"
            });
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
        public async Task<IActionResult> Delete(int id, CancellationToken CT)
        {
            var categoryInDb = _categoryRepository.Get(e => e.Id == id).FirstOrDefault();

            if (categoryInDb is null)
                return NotFound();

            _categoryRepository.Delete(categoryInDb);

            await _categoryRepository.CommitAsync(CT);

            return Ok(new SuccessResponse()
            {
                SuccessNotification = "Delete Category Successfully"
            });
        }
    }
}