using Kino.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Kino.DAL.Repositories;

// базова реалізація IRepository<T> на EF Core.
// SaveChanges тут немає: зберігає зміни лише UnitOfWork, одним викликом для всіх репозиторіїв
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MovieDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(MovieDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await Set.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Set.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await Set.AddAsync(entity);
    }

    public void Update(T entity)
    {
        Set.Update(entity);
    }

    public void Delete(T entity)
    {
        Set.Remove(entity);
    }

    public IQueryable<T> Query()
    {
        return Set.AsQueryable();
    }
}
