namespace CinemaDashboard.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db = new ApplicationDbContext();
    public IActionResult Index(string name, int page = 1, int size = 3)
    {
        var categories = _db.Categories.AsQueryable();

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
    public IActionResult Create(Category category)
    {
        if(!ModelState.IsValid)
            return View(category);
        
        _db.Categories.Add(category);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var category = _db.Categories.SingleOrDefault(c => c.Id == id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost]
    public IActionResult Update(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        _db.Categories.Update(category);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Delete(int id)
    {
        var category = _db.Categories.FirstOrDefault(c => c.Id == id);

        if (category is null) return NotFound();

        _db.Categories.Remove(category);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}
