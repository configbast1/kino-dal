using Kino.DAL.UnitOfWork;
using Kino.PL.Mapping;
using Kino.PL.Models;
using Microsoft.EntityFrameworkCore;

namespace Kino.PL.Services;

// логіка каталогу: правила, робота з постерами, логування.
// до бази звертається лише через IUnitOfWork з DAL - ні DbContext, ні DbSet тут немає
public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPosterStorage _posterStorage;
    private readonly ILogger<MovieService> _logger;

    public MovieService(
        IUnitOfWork unitOfWork,
        IPosterStorage posterStorage,
        ILogger<MovieService> logger)
    {
        _unitOfWork = unitOfWork;
        _posterStorage = posterStorage;
        _logger = logger;
    }

    public string PlaceholderPosterPath => _posterStorage.PlaceholderPath;

    public async Task<IReadOnlyList<MovieDto>> GetAllAsync()
    {
        // сортування виконується в базі (Query() повертає IQueryable)
        var movies = await _unitOfWork.Movies.Query()
            .AsNoTracking()
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title)
            .ToListAsync();

        return movies.Select(m => m.ToDto()).ToList();
    }

    public async Task<MovieDto?> GetByIdAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }

        var movie = await _unitOfWork.Movies.GetByIdAsync(id.Value);

        return movie?.ToDto();
    }

    public Task<bool> ExistsAsync(int id) =>
        _unitOfWork.Movies.Query().AnyAsync(m => m.Id == id);

    public async Task<MovieDto> CreateAsync(MovieDto movie)
    {
        var posterPath = await _posterStorage.SaveAsync(movie.PosterContent, movie.PosterFileName);

        var entity = movie.ToEntity();
        entity.Id = 0;
        entity.PosterPath = posterPath ?? _posterStorage.PlaceholderPath;

        await _unitOfWork.Movies.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Додано фільм {Title} (Id = {Id})", entity.Title, entity.Id);

        return entity.ToDto();
    }

    public async Task<MovieDto?> UpdateAsync(int id, MovieDto movie)
    {
        var existing = await _unitOfWork.Movies.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        var newPosterPath = await _posterStorage.SaveAsync(movie.PosterContent, movie.PosterFileName);
        var previousPosterPath = existing.PosterPath;

        movie.CopyEditableFields(existing);

        if (newPosterPath is not null)
        {
            existing.PosterPath = newPosterPath;
        }

        _unitOfWork.Movies.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        // старий файл видаляємо лише після успішного збереження
        if (newPosterPath is not null)
        {
            _posterStorage.Delete(previousPosterPath);
        }

        _logger.LogInformation("Оновлено фільм {Title} (Id = {Id})", existing.Title, existing.Id);

        return existing.ToDto();
    }

    public async Task<MovieDto?> DeleteAsync(int id)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(id);
        if (movie is null)
        {
            return null;
        }

        _unitOfWork.Movies.Delete(movie);
        await _unitOfWork.SaveChangesAsync();

        _posterStorage.Delete(movie.PosterPath);

        _logger.LogInformation("Видалено фільм {Title} (Id = {Id})", movie.Title, movie.Id);

        return movie.ToDto();
    }

    public async Task<bool> IsTitleUniqueAsync(string? title, int excludeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return true;
        }

        var normalized = title.Trim();

        return !await _unitOfWork.Movies.Query()
            .AnyAsync(m => m.Id != excludeId && m.Title == normalized);
    }

    public bool IsTitleUnique(string? title, int excludeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return true;
        }

        var normalized = title.Trim();

        return !_unitOfWork.Movies.Query()
            .Any(m => m.Id != excludeId && m.Title == normalized);
    }
}
