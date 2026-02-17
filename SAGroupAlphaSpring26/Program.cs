global using SAGroupAlphaSpring26;
global using SAGroupAlphaSpring26.Models;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

// Adding the database for the EF core, and connecting it to the connection string in the appsettings.json file.
// This is the DB that our C# classes will be stored in, and we will use EF core to interact with it.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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
