using Microsoft.EntityFrameworkCore;
using MainPortfolio.Data;

namespace MainPortfolio.Extensions;

public static class MySqlDbExtensions
{
    public static IServiceCollection AddMySqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? mySqlConnectionString = configuration.GetConnectionString("MySqlConnection");

        if (string.IsNullOrEmpty(mySqlConnectionString))
        {
            throw new InvalidOperationException("The connection string 'MySqlRailwayConnection' was not found.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));

        return services;
    }
}
