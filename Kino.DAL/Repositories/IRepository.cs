namespace Kino.DAL.Repositories;

// узагальнений репозиторій: однаковий набір операцій для будь-якої сутності
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    // для власних запитів: фільтрація, сортування, пошук.
    // запит виконується в базі даних, а не в пам'яті
    IQueryable<T> Query();
}
