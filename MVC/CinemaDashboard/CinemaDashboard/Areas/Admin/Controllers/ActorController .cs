

namespace Ecommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ActorController : Controller
{
    private readonly ApplicationDbContext _db = new ApplicationDbContext();
    IFileUpload fileUpload = new FileUpload();
    public IActionResult Index(string name, int page = 1, int size = 3)
    {
        var actors = _db.Actors.AsQueryable();

        if (name is not null)
        {
            actors = actors.Where(c => c.Name.ToLower().Contains(name.ToLower()));
        }
        var total = Math.Ceiling(actors.Count() / (double)size);
        actors = actors.Skip((page - 1) * size).Take(size);


        return View(new ActorWithFilterVM()
        {
            Actors = actors,
            Name = name ?? "",
            TotalPages = total
        });
    }
    [HttpGet]
    public IActionResult Create(Actor actor)
    {
        return View(actor);
    }
    [HttpPost]
    public IActionResult Create(Actor actor,IFormFile Img)
    {
        if(!ModelState.IsValid)
            return View(actor);

        if(Img is not null && Img.Length > 0)
        {
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Actors", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            actor.Img = fileName;
        }
        _db.Actors.Add(actor);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var actor = _db.Actors.SingleOrDefault(c => c.Id == id);
        if (actor is null) return NotFound();
        return View(actor);
    }

    [HttpPost]
    public IActionResult Update(Actor actor,IFormFile Img)
    {
        if (!ModelState.IsValid)
            return View(actor);

        var ActorInDb = _db.Actors.AsNoTracking().Where(e => e.Id == actor.Id).SingleOrDefault();
        if(ActorInDb is null) return NotFound();


        if (Img is not null && Img.Length > 0)
        {
            // create new img 
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Actors", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            if (!string.IsNullOrEmpty(ActorInDb.Img))
            {
                var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Actors", ActorInDb.Img);
                if (oldFilePath != null)
                    fileUpload.DeleteFileLocally(oldFilePath);
            }

            actor.Img = fileName;
        }
        else
        {
            actor.Img = ActorInDb.Img;
        }
        _db.Actors.Update(actor);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Delete(int id)
    {
        var actor = _db.Actors.FirstOrDefault(c => c.Id == id);
        if(actor is null)   return NotFound();

        if (!string.IsNullOrEmpty(actor.Img))
        {
            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Actors", actor.Img);
            if (filePath != null)
            {
                fileUpload.DeleteFileLocally(filePath);
            }
        }

        _db.Actors.Remove(actor);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}
