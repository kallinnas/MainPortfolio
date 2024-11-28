using MainPortfolio.Extensions;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Services.Interfaces;
using MainPortfolio.Services;
using MainPortfolio.Repositories;

var builder = WebApplication.CreateBuilder(args);

// RAILWAY VARIABLES
builder.Configuration.AddEnvironmentVariables();

// CORS
builder.Services.AddAppCors(builder.Configuration);

// JWT configuration
builder.Services.AddJwtAuthWithSwagger(builder.Configuration);

// MySQL Db
builder.Services.AddMySqlDatabase(builder.Configuration);
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<JwtService>();

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







