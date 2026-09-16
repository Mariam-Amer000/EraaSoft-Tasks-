using System.Linq.Expressions;

namespace CinemaDashboard.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity, CancellationToken CT = default);
        bool Update(T entity);
        bool Delete(T entity);
        Task<int> CommitAsync(CancellationToken CT = default);
        IQueryable<T> Get(
       Expression<Func<T, bool>>? expression = null,
       Expression<Func<T, object>>[]? includes = null,
       bool tracked = true);
        T? GetOne(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true);
    }
}
