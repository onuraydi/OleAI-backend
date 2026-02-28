using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Persistence;
using OleAI.Backend.Persistence.Contexts;

var builder = WebApplication.CreateBuilder(args);

// ---- Service Registrations (katman bazlı extension methods) ----
builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

// ---- Middleware Pipeline ----
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

// Automatic Migration at Startup (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
