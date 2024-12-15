using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Security.Interfaces;
using MainPortfolio.Repositories;
using MainPortfolio.Extensions;
using MainPortfolio.Middleware;
using MainPortfolio.Security;

var builder = WebApplication.CreateBuilder(args);

// RAILWAY VARIABLES
builder.Configuration.AddEnvironmentVariables();

// CORS
builder.Services.AddAppCors(builder.Configuration);

// JWT Configuration
builder.Services.AddJwtAuthentication(builder.Configuration);
// Swagger with JWT
builder.Services.AddSwaggerWithJwtAuth();

// Mongo Db
builder.Services.AddMongoDatabase(builder.Configuration);
// MySQL Db
builder.Services.AddMySqlDatabase(builder.Configuration);
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// App services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CvPdfRepository>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>(); // Goes first to handle ex in subsequent middleware

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

app.UseMiddleware<RefreshTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html"); // Fallback to Angular's index.html for client-side routes

app.Run();







