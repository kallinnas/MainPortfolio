namespace MainPortfolio.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration["AllowedOrigins"]?.Split(",") ?? Array.Empty<string>();

        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder.WithOrigins(allowedOrigins)
                   .WithExposedHeaders(configuration["Keys:AccessToken"]!)
                   .AllowCredentials()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        }));

        return services;
    }
}
