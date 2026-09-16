namespace CinemaDashboard.Repositories;

public class BulkRepository<T> : Repository<T>, IBulkRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    public BulkRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public bool DeleteRange(IEnumerable<MovieSubImg> movieSubImgs)
    {
        try
        {
            _context.RemoveRange(movieSubImgs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return false;
        }
    }
}
