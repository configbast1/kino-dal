using Kino.DAL.Data;
using Kino.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kino.DAL;

// реєстрація всього, що стосується доступу до даних.
// PL викликає один метод і не налаштовує EF Core сам
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(
        this IServiceCollection services,
        string? connectionString)
    {
        services.AddDbContext<MovieDbContext>(options =>
            options.UseSqlServer(connectionString));

        // один UnitOfWork (і один DbContext) на HTTP-запит
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }

    // створює базу разом із початковими даними, якщо її ще немає
    public static IServiceProvider EnsureDatabaseCreated(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
        context.Database.EnsureCreated();

        return services;
    }
}
