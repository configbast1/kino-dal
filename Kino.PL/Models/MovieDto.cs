namespace Kino.PL.Models;

// модель, якою MovieService обмінюється з контролером і в'юхами.
// сутність Movie з DAL нагору не піднімається взагалі
public class MovieDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? OriginalTitle { get; set; }

    public string Director { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    public string PosterPath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Rating { get; set; }

    // вміст завантаженого постера.
    // вміст файлу як Stream: так сервісу не потрібен IFormFile
    public Stream? PosterContent { get; set; }

    public string? PosterFileName { get; set; }
}
