﻿using Microsoft.EntityFrameworkCore;
using MainPortfolio.Data;

namespace MainPortfolio.Extensions;

public static class MySqlDbExtension
{
    public static IServiceCollection AddMySqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? mySqlConnectionString = Environment.GetEnvironmentVariable("SIGNALR_MYSQL_CONNECTION_STRING");

        if (string.IsNullOrEmpty(mySqlConnectionString))
        {
            mySqlConnectionString = configuration.GetConnectionString("MySqlConnection");
        }

        if (string.IsNullOrEmpty(mySqlConnectionString))
        {
            throw new InvalidOperationException("The MySQL connection string was not found in environment variables or configuration.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 0, 21)),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure()));

        return services;
    }
}
