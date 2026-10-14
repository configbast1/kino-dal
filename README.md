# 🎬 Кінокаталог — винесення рівня доступу до даних (DAL)

Навчальний ASP.NET Core MVC застосунок «10 найкращих фільмів».
У цьому завданні з однопроєктного застосунку винесено **лише рівень доступу до даних (DAL)**
в окремий проєкт. Рівень бізнес-логіки (BLL) **не** виділявся: сервіси лишаються в PL.

**Стек:** ASP.NET Core MVC (.NET 10) · Entity Framework Core 10 · SQL Server LocalDB · Repository + Unit of Work

## Скріншоти

![Головна сторінка](screenshots/01-home-top.png)
![Сторінка фільму](screenshots/03-details.png)

## Структура рішення

```
Kino.sln
├── Kino.DAL   Class Library     - доступ до даних (EF Core, репозиторії, Unit of Work)
└── Kino.PL    ASP.NET Core MVC  - контролери, в'юхи, валідація, сервіси, постери
```

Залежність одна: **Kino.PL → Kino.DAL**. DAL нічого не знає про PL, ASP.NET Core та `IFormFile`.

## Kino.DAL

| Файл | Що робить |
|---|---|
| `Entities/Movie.cs` | сутність фільму, без атрибутів валідації |
| `Entities/Genre.cs` | довідник жанрів |
| `Entities/Director.cs` | довідник режисерів |
| `Data/MovieDbContext.cs` | контекст EF Core: таблиці `Movies`, `Genres`, `Directors` та початкові дані |
| `Repositories/IRepository.cs` | узагальнений інтерфейс: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `Update`, `Delete`, `Query` |
| `Repositories/Repository.cs` | базова реалізація `IRepository<T>` на EF Core |
| `UnitOfWork/IUnitOfWork.cs` | репозиторії `Movies`, `Genres`, `Directors` та `SaveChangesAsync()` |
| `UnitOfWork/UnitOfWork.cs` | тримає один `MovieDbContext` і створює всі репозиторії на ньому |
| `DependencyInjection.cs` | `AddDataAccessLayer(configuration)` та `EnsureDatabaseCreated()` |

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Movie> Movies { get; }
    IRepository<Genre> Genres { get; }
    IRepository<Director> Directors { get; }
    Task<int> SaveChangesAsync();
}
```

Репозиторії самі нічого не зберігають: `SaveChangesAsync()` викликається один раз на `UnitOfWork`,
тому всі зміни в межах запиту потрапляють у базу разом.

## Kino.PL

| Папка | Що там |
|---|---|
| `Controllers/` | `MoviesController`, `HomeController` — працюють лише з `IMovieService` |
| `Services/` | `MovieService` (працює через `IUnitOfWork`), `PosterStorage` (файли в `wwwroot`) |
| `Models/`, `Mapping/` | `MovieDto` та маппінг `Movie` ↔ `MovieDto` |
| `ViewModels/` | `MovieViewModel` — модель форми з атрибутами валідації та `IFormFile` |
| `Validation/` | кастомні атрибути валідації |
| `Views/`, `wwwroot/` | розмітка, стилі, постери |

Реєстрація в `Program.cs`:

```csharp
builder.Services.AddDataAccessLayer(builder.Configuration);   // MovieDbContext + IUnitOfWork
builder.Services.AddScoped<IPosterStorage, PosterStorage>();
builder.Services.AddScoped<IMovieService, MovieService>();
```

## Запуск

1. Відкрити `Kino.sln` у Visual Studio.
2. Стартовий проєкт — `Kino.PL`, натиснути F5.

База `MovieCatalogDalDb` (LocalDB) створюється автоматично під час першого запуску
разом із 10 фільмами, жанрами та режисерами.
