var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve wwwroot files (Angular dist)
app.UseRouting();

app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html"); // Fallback to Angular's index.html for client-side routes

app.Run();
