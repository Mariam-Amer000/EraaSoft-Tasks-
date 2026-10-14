using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE},{RoleConstants.CUSTOMER}")]
        public IActionResult Get(string? name)
        {
            var categories = _categoryRepository.Get();

            if (name is not null)
            {
                categories = categories.Where(c => c.Name.Contains(name));
            }

            return Ok(categories);
        }
        [HttpGet]
        [Route(nameof(GetOne))]
        [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE},{RoleConstants.CUSTOMER}")]
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

            if (result)
                await _categoryRepository.CommitAsync(CT);

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

