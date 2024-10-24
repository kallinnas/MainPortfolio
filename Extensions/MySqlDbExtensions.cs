﻿using Microsoft.EntityFrameworkCore;
using MainPortfolio.Data;
using Microsoft.Extensions.Options;

namespace MainPortfolio.Extensions;

public static class MySqlDbExtensions
{
    public static IServiceCollection AddMySqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? mySqlConnectionString = configuration.GetConnectionString("MySqlRailwayConnection");

        if (string.IsNullOrEmpty(mySqlConnectionString))
        {
            throw new InvalidOperationException("The connection string 'MySqlRailwayConnection' was not found.");
        }

        //services.AddDbContext<AppDbContext>(options =>
        ////options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));
        //options.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 0, 21))));
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 0, 21)),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure()));  // Enable retry on transient failures

        return services;
    }
}
