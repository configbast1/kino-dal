using Kino.DAL;
using Kino.PL.Extensions;
using Kino.PL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// рівень доступу до даних: MovieDbContext + IUnitOfWork (усе налаштовано всередині Kino.DAL)
builder.Services.AddDataAccessLayer(
    builder.Configuration.GetConnectionString("DefaultConnection"));

// сервіси рівня представлення
builder.Services.AddScoped<IPosterStorage, PosterStorage>();
builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

app.Services.EnsureDatabaseCreated();
app.UseApplicationLocalization();
app.UseApplicationErrorHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
