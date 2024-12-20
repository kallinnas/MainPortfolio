﻿using MainPortfolio.Data;
using Microsoft.EntityFrameworkCore;

namespace MainPortfolio.Extensions;

public static class MigrationDbExtension
{
    public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure migrations only apply if necessary
            if (!dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }

        return app;
    }
}
