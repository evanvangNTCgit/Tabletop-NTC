global using SAGroupAlphaSpring26;
global using SAGroupAlphaSpring26.Models;
global using SAGroupAlphaSpring26.ViewModels;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

// Adding the database for the EF core, and connecting it to the connection string in the appsettings.json file.
// This is the DB that our C# classes will be stored in, and we will use EF core to interact with it.
// Using MySQL for local development
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

var app = builder.Build();

// Creates mock data for production only if it doesn't already exist.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Should run migrations for mock data.
    //context.Database.Migrate();

    // Seed data for LOCAL DEVELOPMENT
    if (app.Environment.IsDevelopment())
    {
        // Seed PieceTypes if none exist
        if (!context.PieceTypes.Any())
        {
            context.PieceTypes.AddRange(
                new PieceType { Name = "Player" },
                new PieceType { Name = "Map" },
                new PieceType { Name = "Structure" },
                new PieceType { Name = "Object" },
                new PieceType { Name = "Goblin" },
                new PieceType { Name = "Orc" },
                new PieceType { Name = "Shop" }
            );
            context.SaveChanges();
        }

        // Seed Pieces if none exist - use references to PieceTypes
        if (!context.Pieces.Any())
        {
            var playerType = context.PieceTypes.First(pt => pt.Name == "Player");
            var mapType = context.PieceTypes.First(pt => pt.Name == "Map");
            var objectType = context.PieceTypes.First(pt => pt.Name == "Object");
            var goblinType = context.PieceTypes.First(pt => pt.Name == "Goblin");

            context.Pieces.AddRange(
                new Piece { PieceTypeID = mapType.Id, Name = "Default Dungeon", ImagePath = "/images/testMap.png", Price = 0.00m },
                new Piece { PieceTypeID = playerType.Id, Name = "Cleric", ImagePath = "/images/Cleric.png", Price = 0.00m },
                new Piece { PieceTypeID = goblinType.Id, Name = "Goblin Chief", ImagePath = "/images/GoblinChief.png", Price = 0.00m },
                new Piece { PieceTypeID = objectType.Id, Name = "Basic Chest", ImagePath = "/images/chest.png", Price = 0.00m }
            );
            context.SaveChanges();
        }

        // Seed local user if not exists
        if (!context.Users.Any(u => u.Username == "LocalDM"))
        {
            var localUser = new User { Username = "LocalDM", Email = "local@demo.com" };
            context.Users.Add(localUser);
            context.SaveChanges();

            // Get the seeded pieces for tokens
            var mapPiece = context.Pieces.First(p => p.Name == "Default Dungeon");
            var clericPiece = context.Pieces.First(p => p.Name == "Cleric");
            var goblinPiece = context.Pieces.First(p => p.Name == "Goblin Chief");
            var chestPiece = context.Pieces.First(p => p.Name == "Basic Chest");

            // Create a session for the local user
            var localSession = new Session { UserId = localUser.Id, Notes = "Local Test Session", LastUpdated = DateTime.Now };
            context.Sessions.Add(localSession);
            context.SaveChanges();

            // Add tokens for the map (Map piece + tokens)
            context.Tokens.AddRange(
                new Token { SessionId = localSession.Id, PieceID = mapPiece.Id, Name = "Default Dungeon", X = 0, Y = 0, ZIndex = 0, Visibility = true },
                new Token { SessionId = localSession.Id, PieceID = clericPiece.Id, Name = "Cleric", X = 50, Y = 15, ZIndex = 3, Visibility = true },
                new Token { SessionId = localSession.Id, PieceID = goblinPiece.Id, Name = "Goblin Chief", X = 50, Y = 5, ZIndex = 1, Visibility = true },
                new Token { SessionId = localSession.Id, PieceID = chestPiece.Id, Name = "Basic Chest", X = 50, Y = 10, ZIndex = 2, Visibility = false }
            );
            context.SaveChanges();
        }
    }

    // Production seed data (Azure)
    if (!app.Environment.IsDevelopment())
    {
        if (!context.Users.Any(u => u.Username == "AzureDemoDM"))
        {
            var azureUser = new User { Username = "AzureDemoDM", Email = "demo@live.com"};
            context.Users.Add(azureUser);
            context.SaveChanges();

            // var azureSession = new Session { UserId = azureUser.UserId, Notes = "Live Azure Production Map", LastUpdated = DateTime.Now };
            var azureSession = new Session { UserId = azureUser.Id, Notes = string.Empty, LastUpdated = DateTime.Now, Id = 1, User = azureUser };
            context.Sessions.Add(azureSession);
            context.SaveChanges();

            context.Tokens.AddRange(
                new Token { SessionId = azureSession.Id, PieceID = 1, Name = "Production Map", X = 0, Y = 0, ZIndex = 0, Visibility = true },
                new Token { SessionId = azureSession.Id, PieceID = 2, Name = "Production Goblin", X = 200, Y = 200, ZIndex = 1, Visibility = true }
            );
            context.SaveChanges();
        }
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
