using MainPortfolio.Extensions;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Repositories;
using MainPortfolio.Security.Services;
using MainPortfolio.Security.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// RAILWAY VARIABLES
builder.Configuration.AddEnvironmentVariables();

// CORS
builder.Services.AddAppCors(builder.Configuration);

// JWT configuration
builder.Services.AddJwtAuthWithSwagger(builder.Configuration);

// MySQL Db
builder.Services.AddMySqlDatabase(builder.Configuration);
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// App services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDatabase(); // Apply pending migrations (for railway migration db)

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve wwwroot files (Angular dist)
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html"); // Fallback to Angular's index.html for client-side routes

app.Run();







