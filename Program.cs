using EventEaseProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Force use of live connection string, ignoring environment
builder.Services.AddDbContext<EEDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("LiveConn");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("LiveConn connection string is not configured.");
    }
    options.UseSqlServer(connectionString);
});

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Disable development-specific middleware
app.UseExceptionHandler("/Home/Error"); // Use error handling for all environments
app.UseHsts(); // Enforce HSTS for security

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files (e.g., images for Venue)
app.UseRouting();
app.UseAuthorization();

// Configure controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Additional route for Venues, Events, and Bookings
app.MapControllerRoute(
    name: "entity",
    pattern: "{controller=Venues}/{action=Index}/{id?}");

app.Run();