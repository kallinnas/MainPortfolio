using MongoDB.Driver;

namespace MainPortfolio.Extensions;

public static class MongoDbExtension
{
    public static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

        if (string.IsNullOrEmpty(settings!.ConnectionString) || string.IsNullOrEmpty(settings.DatabaseName))
        {
            throw new InvalidOperationException("MongoDB connection details are missing.");
        }

        services.AddSingleton<IMongoClient>(new MongoClient(settings.ConnectionString));
        services.AddSingleton<IMongoDatabase>(sp => sp.GetService<IMongoClient>()!.GetDatabase(settings.DatabaseName));

        return services;
    }
}

public class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}
