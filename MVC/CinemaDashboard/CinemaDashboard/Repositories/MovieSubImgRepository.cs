namespace CinemaDashboard.Repositories;

public class MovieSubImgRepository : Repository<MovieSubImg>
{
    public bool DeleteRange(IEnumerable<MovieSubImg> movieSubImgs)
    {
        try
        {
            _db.RemoveRange(movieSubImgs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return false;
        }
    }
}
