namespace CinemaDashboard.Areas.Admin.Controllers;

[Area("Admin")]
public class MovieController : Controller
{
    private readonly ApplicationDbContext _db = new ApplicationDbContext();
    IFileUpload fileUpload = new FileUpload();
    public IActionResult Index(MovieFilter filter, int page = 1, int size = 3)
    {
        var movies = _db.Movies
            .Include(e => e.Cinema)
            .Include(e => e.Category)
            .AsQueryable();
        var cinemas = _db.Cinemas.AsQueryable();
        var categories = _db.Categories.AsQueryable();

        if (filter.Name is not null)
        {
            movies = movies.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if(filter.Maxprice is not null)
        {
            movies = movies.Where(c => c.Price <= filter.Maxprice);
        }
        if(filter.Minprice is not null)
        {
            movies = movies.Where(c => c.Price >= filter.Minprice);
        }
        if (filter.Status is not null)
        {
            movies = movies.Where(c => c.Status == filter.Status);
        }
        if (filter.DateTime is not null)
        {
            movies = movies.Where(c => c.DateTime == filter.DateTime);
        }
        if (filter.CategoryId is not null)
        {
            movies = movies.Where(c => c.CategoryId == filter.CategoryId);
        }
        if (filter.CinemaId is not null)
        {
            movies = movies.Where(c => c.CinemaId == filter.CinemaId);
        }
        var total = Math.Ceiling(movies.Count() / (double)size);
        movies = movies.Skip((page - 1) * size).Take(size);


        return View(new MovieWithFilterVM()
        {
            Movies = movies,
            Categories = categories,
            Cinemas = cinemas,
            Name = filter.Name ?? "",
            TotalPages = total
        });
    }
    [HttpGet]
    public IActionResult Create()
    {
        var categories = _db.Categories.AsQueryable();
        var cinemas = _db.Cinemas.AsQueryable();
        return View(new MovieWithDetalies()
        {
           Categories = categories,
           Cinemas= cinemas,
        });
    }
    [HttpPost]
    public IActionResult Create(Movie movie, IFormFile Img, List<IFormFile> subImgs)
    {
        if(ModelState.IsValid)
        {
            return View(new MovieWithDetalies
            {
                Movie = movie,
                Categories = _db.Categories.ToList(),
                Cinemas = _db.Cinemas.ToList()
            });
        }
        if (Img is not null && Img.Length > 0)
        {
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\MainImg", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            movie.MainImg = fileName;
        }
        _db.Movies.Add(movie);
        _db.SaveChanges();

        if (subImgs.Any())
        {
            // create in wwwroot
            foreach(var item in subImgs)
            {
                var fileName = fileUpload.GenerateFileName(item.FileName);
                if (fileName is null) return BadRequest();

                var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs", fileName);
                if (filePath is null) return BadRequest();

                fileUpload.UploadFileLocally(filePath, item);

                // create in db
                _db.MovieSubImgs.Add(new()
                {
                    SubImg = fileName,
                    MovieId = movie.Id
                });

            }

        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var movie = _db.Movies.SingleOrDefault(c => c.Id == id);
        if (movie is null) return NotFound();

        var categories = _db.Categories.AsQueryable();
        var cinemas = _db.Cinemas.AsQueryable();


        return View(new MovieWithDetalies()
        {
            Movie = movie,
            Categories = categories,
            Cinemas = cinemas,
        });
    }

    [HttpPost]
    public IActionResult Update(Movie movie,IFormFile Img ,List<IFormFile> subImgs)
    {
        if(!ModelState.IsValid)
        {
            return View(new MovieWithDetalies
            {
                Movie = movie,
                Categories = _db.Categories.ToList(),
                Cinemas = _db.Cinemas.ToList()
            });
        }
        var MovieInDb = _db.Movies.AsNoTracking().Where(e => e.Id == movie.Id).SingleOrDefault();
        if(MovieInDb is null) return NotFound();


        if (Img is not null && Img.Length > 0)
        {
            // create new img 
            var fileName = fileUpload.GenerateFileName(Img.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, Img);

            var oldFilePath= fileUpload.GenerateFullPath(FileType.Img, "Movies", MovieInDb.MainImg);
            //delete old img form wwwroot
            fileUpload.DeleteFileLocally(oldFilePath);

            movie.MainImg = fileName;
        }
        else
        {
            movie.MainImg = MovieInDb.MainImg;
        }

        if (subImgs.Any())
        {
            // delete old imgs form wwwroot and DB
            var OldSubImgs = _db.MovieSubImgs.Where(e => e.MovieId == movie.Id);

            foreach (var item in OldSubImgs)
            {
                var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs", item.SubImg);
                if (oldFilePath is null) return BadRequest();

                fileUpload.DeleteFileLocally(oldFilePath);
            }
            _db.MovieSubImgs.RemoveRange(OldSubImgs);

            // create in wwwroot
            foreach (var item in subImgs)
            {
                var fileName = fileUpload.GenerateFileName(item.FileName);
                if (fileName is null) return BadRequest();

                var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs", fileName);
                if (filePath is null) return BadRequest();

                fileUpload.UploadFileLocally(filePath, item);

                // create in db
                _db.MovieSubImgs.Add(new()
                {
                    SubImg = fileName,
                    MovieId = movie.Id
                });

            }

        }
        _db.Movies.Update(movie);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Delete(int id)
    {
        var movie = _db.Movies.FirstOrDefault(c => c.Id == id);
        if(movie is null)   return NotFound();

        var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\MainImg", movie.MainImg);
        if (filePath is null) return BadRequest();

        fileUpload.DeleteFileLocally(filePath);




        var OldSubImgs = _db.MovieSubImgs.Where(e => e.MovieId == movie.Id);
        foreach(var item in OldSubImgs)
        {
            var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs",item.SubImg);
            if (oldFilePath is null) return BadRequest();

            fileUpload.DeleteFileLocally(oldFilePath);
        }
        _db.MovieSubImgs.RemoveRange(OldSubImgs);
        _db.Movies.Remove(movie);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}
