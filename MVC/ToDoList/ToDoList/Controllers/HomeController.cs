using Lecture_21.Helper;

namespace ToDoList.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db = new ApplicationDbContext();
    FileUpload fileUpload = new FileUpload();
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string name)
    {
        User? user = _db.Users
            .SingleOrDefault(u => u.Name == name);

        if (user is null)
        {
            user = new User()
            {
                Name = name
            };

            _db.Users.Add(user);
            _db.SaveChanges();
        }
        return RedirectToAction(nameof(Items), new { id = user.Id });
    }
    public IActionResult Items(int id)
    {
        var items = _db.Items
         .Where(i => i.UserId == id)
         .ToList();

        User? user = _db.Users
          .SingleOrDefault(u => u.Id == id);

        if (user is null)
            return NotFound();


        return View(new UserWithItemsVM()
        {
            User= user,
            Items = items
        });
    }

    [HttpGet]
    public IActionResult CreateItem(int userId)
    {
        User? user = _db.Users.SingleOrDefault(u => u.Id == userId);

        if (user is null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    public IActionResult CreateItem(Item item, int userId, IFormFile? file)
    {
        item.UserId = userId;

        if (file is not null)
        {
            string fileName = fileUpload.GenerateFileName(file);

            string filePath = fileUpload.GeneratePath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            item.FilePath = fileName;
        }

        _db.Items.Add(item);
        _db.SaveChanges();

        return RedirectToAction(nameof(Items), new { id = userId });
    }

    [HttpGet]
    public IActionResult UpdateItem(int id)
    {
        var item = _db.Items
            .SingleOrDefault(e => e.Id == id);

        if (item is null)
            return NotFound();

        return View(item);
    }

    [HttpPost]
    public IActionResult UpdateItem(Item item, IFormFile? file)
    {
        var existingItem = _db.Items
            .SingleOrDefault(i => i.Id == item.Id);

        if (existingItem is null)
            return NotFound();

        if (file is not null)
        {
            string fileName = fileUpload.GenerateFileName(file);

            string filePath = fileUpload.GeneratePath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            existingItem.FilePath = fileName;
        }

        existingItem.Title = item.Title;
        existingItem.Description = item.Description;
        existingItem.Deadline = item.Deadline;

        _db.SaveChanges();

        return RedirectToAction(nameof(Items), new { id = existingItem.UserId });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public IActionResult DeleteItem(int id)
    {
        var item = _db.Items
            .SingleOrDefault(e => e.Id == id);

        if (item is null)
            return NotFound();

        _db.Items.Remove(item);
        _db.SaveChanges();
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Download(int id)
    {
        var item = _db.Items
            .SingleOrDefault(e => e.Id == id);

        if (item is null)
            return NotFound();

        string filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Files",
            item.FilePath!);

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/octet-stream", item.FilePath);
    }
}
/*
 * i should make new table because there are multivalued items
 * so i should make new table and join it with user 
 */