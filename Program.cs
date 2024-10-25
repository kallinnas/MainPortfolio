using MainPortfolio.Extensions;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Services.Interfaces;
using MainPortfolio.Services;
using MainPortfolio.Repositories;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options => options.AddPolicy("AllowAllHeaders", builder =>
{ builder.WithOrigins("https://mainportfolio-production-8e90.up.railway.app").AllowCredentials().AllowAnyHeader().AllowAnyMethod(); }));
//{ builder.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyHeader().AllowAnyMethod(); }));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MySQL Db
builder.Services.AddMySqlDatabase(builder.Configuration);
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<RefreshTokenService>();
// JWT configuration
builder.Services.AddJwtAuthentication(builder.Configuration);
// Swagger with JWT
builder.Services.AddSwaggerWithJwtAuth();

// App services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllHeaders");

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve wwwroot files (Angular dist)
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html"); // Fallback to Angular's index.html for client-side routes

app.Run();







// Apply pending migrations (for railway migration db)
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate(); // Apply any pending migrations
//}