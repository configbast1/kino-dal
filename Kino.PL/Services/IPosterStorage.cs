namespace Kino.PL.Services;

// абстракція сховища постерів.
// реалізація (PosterStorage) працює з wwwroot, тому лишається в PL
public interface IPosterStorage
{
    string PlaceholderPath { get; }

    Task<string?> SaveAsync(Stream? content, string? originalFileName);

    void Delete(string? posterPath);
}
