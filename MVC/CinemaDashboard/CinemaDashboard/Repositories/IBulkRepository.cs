namespace CinemaDashboard.Repositories;

public interface IBulkRepository<T> : IRepository<T> where T : class
{
    bool DeleteRange(IEnumerable<MovieSubImg> movieSubImgs);
}
