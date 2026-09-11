

namespace Ecommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ActorController : Controller
{
    private readonly Repository<Actor> _repository = new();
    IFileUpload fileUpload = new FileUpload();
    public IActionResult Index(string name, int page = 1, int size = 3)
    {
        var actors = _repository.Get();

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
    public async Task<IActionResult> Create(Actor actor,IFormFile Img,CancellationToken CT)
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
        await _repository.CreateAsync(actor, CT);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var actor = _repository.GetOne(c => c.Id == id);
        if (actor is null) return NotFound();
        return View(actor);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Actor actor,IFormFile Img, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return View(actor);

        var ActorInDb = _repository.GetOne(e => e.Id == actor.Id, tracked: false);
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
         _repository.Update(actor);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(int id,CancellationToken CT)
    {
        var actor = _repository.GetOne(c => c.Id == id);
        if(actor is null)   return NotFound();

        if (!string.IsNullOrEmpty(actor.Img))
        {
            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Actors", actor.Img);
            if (filePath != null)
            {
                fileUpload.DeleteFileLocally(filePath);
            }
        }

       _repository.Delete(actor);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
}
