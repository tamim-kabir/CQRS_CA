using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;

namespace Persistence.Configarations;
public static class DbContextConfigaration
{
    public static IServiceCollection DatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            s => s.UseSqlServer(configuration.GetConnectionString("ConnectionString"), 
                m => m.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .EnableSensitiveDataLogging());
        return services;
    }
    public static IServiceProvider ConfigureServices(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database =scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //database.Database.Migrate();
        return services;
    }
}