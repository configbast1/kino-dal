using Kino.DAL.Data;
using Kino.DAL.Entities;
using Kino.DAL.Repositories;

namespace Kino.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly MovieDbContext _context;
    private bool _disposed;

    public UnitOfWork(MovieDbContext context)
    {
        _context = context;

        // усі репозиторії отримують той самий контекст
        Movies = new Repository<Movie>(context);
        Genres = new Repository<Genre>(context);
        Directors = new Repository<Director>(context);
    }

    public IRepository<Movie> Movies { get; }

    public IRepository<Genre> Genres { get; }

    public IRepository<Director> Directors { get; }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _context.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
