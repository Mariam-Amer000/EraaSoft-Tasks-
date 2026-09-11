namespace CinemaDashboard.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    //private readonly ApplicationDbContext _db = new ApplicationDbContext();
    private readonly Repository<Category> _repository = new();
    public IActionResult Index(string name, int page = 1, int size = 3)
    {
        var categories = _repository.Get();

        if (name is not null)
        {
            categories = categories.Where(c => c.Name.ToLower().Contains(name.ToLower()));
        }
        var total = Math.Ceiling(categories.Count() / (double)size);
        categories = categories.Skip((page - 1) * size).Take(size);


        return View(new CategoryWithFilterVM()
        {
            Categories = categories,
            Name = name ?? "",
            TotalPages = total
        });
    }
    [HttpGet]
    [ActionName("Create")]
    public IActionResult CreateView(Category category)
    {
        return View(category);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Category category,CancellationToken CT)
    {
        if(!ModelState.IsValid)
            return View(category);

        await _repository.CreateAsync(category,CT);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var category = _repository.GetOne(c => c.Id == id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Category category, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return View(category);

        _repository.Update(category);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(int id, CancellationToken CT)
    {
        var category = _repository.GetOne(c => c.Id == id);

        if (category is null) return NotFound();

        _repository.Delete(category);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
}
