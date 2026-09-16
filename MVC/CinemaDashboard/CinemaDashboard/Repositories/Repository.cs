using System.Linq.Expressions;

namespace CinemaDashboard.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _db;
   public Repository(ApplicationDbContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }
    //CRUD
    public async Task<bool> CreateAsync(T entity, CancellationToken CT = default)
    {
        try
        {
            await _db.AddAsync(entity, CT);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return false;
        }
    }
    public bool Update(T entity)
    {
        try
        {
            _db.Update(entity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return false;
        }
    }
    public bool Delete(T entity)
    {
        try
        {
            _db.Remove(entity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return false;
        }
    }
    public async Task<int> CommitAsync(CancellationToken CT = default)
    {
        try
        {
            return await _context.SaveChangesAsync(CT);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            return 0;
        }
    }
    public IQueryable<T> Get(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true)
    {

        var entities = _db.AsQueryable();

        if (expression is not null)
            entities = entities.Where(expression);

        if (includes is not null && includes.Any())
        {
            foreach (var item in includes)
            {
                if (item is not null)
                    entities = entities.Include(item);
            }
        }

        if (!tracked)
            entities = entities.AsNoTracking();

        return entities;
    }

    public T? GetOne(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes=null,
        bool tracked = true)
    {
        return Get(expression,includes,tracked).FirstOrDefault();
    }
}
