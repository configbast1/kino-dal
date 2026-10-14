using Kino.DAL.Entities;
using Kino.DAL.Repositories;

namespace Kino.DAL.UnitOfWork;

// одна точка доступу до всіх репозиторіїв.
// усі вони працюють з одним MovieDbContext, тому зміни зберігаються разом
public interface IUnitOfWork : IDisposable
{
    IRepository<Movie> Movies { get; }

    IRepository<Genre> Genres { get; }

    IRepository<Director> Directors { get; }

    Task<int> SaveChangesAsync();
}
