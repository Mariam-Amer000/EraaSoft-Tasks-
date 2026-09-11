namespace Ecommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class CinemaController : Controller
{
    //private readonly ApplicationDbContext _db = new ApplicationDbContext();
    private readonly Repository<Cinema> _repository = new();
    IFileUpload fileUpload = new FileUpload();
    public IActionResult Index(string name, int page = 1, int size = 3)
    {
        var cinemas = _repository.Get();

        if (name is not null)
        {
            cinemas = cinemas.Where(c => c.Name.ToLower().Contains(name.ToLower()));
        }
        var total = Math.Ceiling(cinemas.Count() / (double)size);
        cinemas = cinemas.Skip((page - 1) * size).Take(size);


        return View(new CinemaWithFilterVM()
        {
            Cinemas = cinemas,
            Name = name ?? "",
            TotalPages = total
        });
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Cinema cinema,IFormFile Img,CancellationToken CT)
    {
        if(!ModelState.IsValid)
            return View(cinema);

        if (Img is not null && Img.Length > 0)
        {
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Cinemas", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            cinema.Img = fileName;
        }
        await _repository.CreateAsync(cinema, CT);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var cinema = _repository.GetOne(c => c.Id == id);
        if (cinema is null) return NotFound();
        return View(cinema);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Cinema cinema,IFormFile Img, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return View(cinema);

        var CinemaInDb = _repository.GetOne(e => e.Id == cinema.Id, tracked: false);
        if(CinemaInDb is null) return NotFound();


        if (Img is not null && Img.Length > 0)
        {
            // create new img 
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Cinemas", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            if (!string.IsNullOrEmpty(CinemaInDb.Img))
            {
                var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Cinemas", CinemaInDb.Img);
                if (oldFilePath != null)
                    fileUpload.DeleteFileLocally(oldFilePath);
            }

            cinema.Img = fileName;
        }
        else
        {
            cinema.Img = CinemaInDb.Img;
        }
       _repository.Update(cinema);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(int id,CancellationToken CT)
    {
        var cinema = _repository.GetOne(c => c.Id == id);
        if(cinema is null)   return NotFound();

        if (!string.IsNullOrEmpty(cinema.Img))
        {
            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Cinemas", cinema.Img);
            if (filePath != null)
                fileUpload.DeleteFileLocally(filePath);
        }

        _repository.Delete(cinema);
        await _repository.CommitAsync(CT);
        return RedirectToAction("Index");
    }
}
