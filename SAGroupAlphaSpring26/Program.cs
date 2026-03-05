global using SAGroupAlphaSpring26;
global using SAGroupAlphaSpring26.Models;
global using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

// Adding the database for the EF core, and connecting it to the connection string in the appsettings.json file.
// This is the DB that our C# classes will be stored in, and we will use EF core to interact with it.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

var app = builder.Build();

// Creates mock data for production only if it doesn't already exist.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Should run migrations for mock data.
    context.Database.Migrate();

    if (!context.Users.Any(u => u.Username == "AzureDemoDM"))
    {
        var azureUser = new User { Username = "AzureDemoDM", Email = "demo@live.com" };
        context.Users.Add(azureUser);
        context.SaveChanges();

        // var azureSession = new Session { UserId = azureUser.UserId, Notes = "Live Azure Production Map", LastUpdated = DateTime.Now };
        var azureSession = new Session { UserId = azureUser.UserId, Notes = string.Empty, LastUpdated = DateTime.Now, SessionId = 0, User = azureUser };
        context.Sessions.Add(azureSession);
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
