

using CinemaDashboard.Utility;

namespace CinemaDashboard.Areas.Admin.Controllers;

[Area("Admin")]
public class MovieController : Controller
{
    private readonly IRepository<Movie> _movieRepository ;
    private readonly IBulkRepository<MovieSubImg> _movieSubImgRepository ;
    private readonly IRepository<Category> _categoryRepository ;
    private readonly IRepository<Cinema> _cinemaRepository;
     public MovieController(
         IRepository<Movie> movieRepository,
         IBulkRepository<MovieSubImg> movieSubImgRepository,
         IRepository<Category> categoryRepository,
         IRepository<Cinema> cinemaRepository)
    {
        _movieRepository = movieRepository ;
        _movieSubImgRepository = movieSubImgRepository ;
        _categoryRepository= categoryRepository ;
        _cinemaRepository = cinemaRepository;
    }

    IFileUpload fileUpload = new FileUpload();
    public IActionResult Index(MovieFilter filter, int page = 1, int size = 3)
    {
        var movies = _movieRepository.Get(includes: [e => e.Cinema, e => e.Category]);
        var cinemas = _cinemaRepository.Get();
        var categories = _categoryRepository.Get();

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
        var cinemas = _cinemaRepository.Get();
        var categories = _categoryRepository.Get();
        return View(new MovieWithDetalies()
        {
           Categories = categories,
           Cinemas= cinemas,
        });
    }
    [HttpPost]
    public async Task<IActionResult> Create(Movie movie, IFormFile Img, List<IFormFile> subImgs,CancellationToken CT)
    {
        if(ModelState.IsValid)
        {
            return View(new MovieWithDetalies
            {
                Movie = movie,
                Categories = _categoryRepository.Get(),
                Cinemas = _cinemaRepository.Get()
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
        await _movieRepository.CreateAsync(movie);
        await _movieRepository.CommitAsync(CT);

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
                await _movieSubImgRepository.CreateAsync(new()
                {
                    SubImg = fileName,
                    MovieId = movie.Id
                });

            }

        }
        await _movieRepository.CommitAsync();
        TempData[NotificationConstant.SUCCESS_NOTIFICATION] = "Create Movie Successfuly";
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        var movie = _movieRepository.GetOne(c => c.Id == id);
        if (movie is null) return NotFound();

        var cinemas = _cinemaRepository.Get();
        var categories = _categoryRepository.Get();


        return View(new MovieWithDetalies()
        {
            Movie = movie,
            Categories = categories,
            Cinemas = cinemas,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Update(Movie movie,IFormFile Img ,List<IFormFile> subImgs,CancellationToken CT)
    {
        if(!ModelState.IsValid)
        {
            return View(new MovieWithDetalies
            {
                Movie = movie,
                Categories = _categoryRepository.Get(),
                Cinemas = _cinemaRepository.Get()
            });
        }
        var MovieInDb = _movieRepository.GetOne(e => e.Id == movie.Id);
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
            var OldSubImgs = _movieSubImgRepository.Get(e => e.MovieId == movie.Id);

            foreach (var item in OldSubImgs)
            {
                var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs", item.SubImg);
                if (oldFilePath is null) return BadRequest();

                fileUpload.DeleteFileLocally(oldFilePath);
            }
            _movieSubImgRepository.DeleteRange(OldSubImgs);

            // create in wwwroot
            foreach (var item in subImgs)
            {
                var fileName = fileUpload.GenerateFileName(item.FileName);
                if (fileName is null) return BadRequest();

                var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs", fileName);
                if (filePath is null) return BadRequest();

                fileUpload.UploadFileLocally(filePath, item);

                // create in db
                await _movieSubImgRepository.CreateAsync(new()
                {
                    SubImg = fileName,
                    MovieId = movie.Id
                });
            }

        }
        _movieRepository.Update(movie);
        await _movieRepository.CommitAsync();
        TempData[NotificationConstant.SUCCESS_NOTIFICATION] = "Update Movie Successfuly";
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(int id)
    {
        var movie = _movieRepository.GetOne(c => c.Id == id);
        if(movie is null)   return NotFound();

        var filePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\MainImg", movie.MainImg);
        if (filePath is null) return BadRequest();

        fileUpload.DeleteFileLocally(filePath);




        var OldSubImgs = _movieSubImgRepository.Get(e => e.MovieId == movie.Id);
        foreach(var item in OldSubImgs)
        {
            var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "Movies\\SubImgs",item.SubImg);
            if (oldFilePath is null) return BadRequest();

            fileUpload.DeleteFileLocally(oldFilePath);
        }
        _movieSubImgRepository.DeleteRange(OldSubImgs);
       _movieRepository.Delete(movie);
        await _movieRepository.CommitAsync();
        TempData[NotificationConstant.SUCCESS_NOTIFICATION] = "Delete Movie Successfuly";
        return RedirectToAction("Index");
    }
}
