using Microsoft.EntityFrameworkCore;

namespace SAGroupAlphaSpring26.Data
{
    // The data context class.
    // serves as a bridge between your domain model (your C# classes) and the underlying database. 
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }


        /*
        * property in your DbContext class generally corresponds to a table in the database, where TEntity is the C# class (model) that maps to the table's structure.
        */

        public DbSet<User> Users { get; set; }

        public DbSet<Session> Sessions { get; set; }

        // So this is a reference to a PieceTypes table.
        public DbSet<PieceType> PieceTypes
        {
            get; set;
        }
        public DbSet<Piece> Pieces { get; set; }

        public DbSet<Set> Sets { get; set; }

        // Removed TokenCordinates, I think it will be easier to have one Token DB...
        public DbSet<Token> Tokens { get; set; }


        public DbSet<Collection> Collections { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public DbSet<Receipt> Receipts { get; set; }

        public DbSet<Store> Marketplaces { get; set; }


        // We overide the on model creation method
        // BECAUSE HERE... we specify what relationships we want to have in our database, and how we want to structure our tables.
        // Like Pieces and piecetypes have a relationship, so we would specify that here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //        // I will make some test piece types then comment it out...
            //        modelBuilder.Entity<PieceType>().HasData(
            //            new PieceType { Id = 1, Name = "Enemy" },
            //            new PieceType { Id = 2, Name = "Map" }
            //        );

            //        //user
            //        modelBuilder.Entity<User>().HasData(
            //        new User { Id = 1, Username = "Fred", CreatedAt = DateTime.Now, Email = "tjackson@students.ntc.edu",}
            //);

            //        // sets
            //        modelBuilder.Entity<Set>().HasData(
            //        new Set { Id = 1, Name = "Base Set", Price = 0.00m,}
            //);

            //        // pieces
            //        modelBuilder.Entity<Piece>().HasData(
            //        new Piece { Id = 1, PieceTypeID = 2, SetID = 1,  Name = "Default Dungeon", ImagePath = "/images/default.png", Price = 0.00m },
            //        new Piece { Id = 2, PieceTypeID = 1, SetID = 1, Name = "Goblin", ImagePath = "/images/goblin.png", Price = 0.00m },
            //        new Piece { Id = 3, PieceTypeID = 1, SetID = 1, Name = "Orc", ImagePath = "/images/hero-knight.png", Price = 0.00m },
            //        new Piece { Id = 4, PieceTypeID = 1, SetID = 1, Name = "Knight", ImagePath = "/images/hero-knight.png", Price = 0.00m }
            //);

            //        // Session
            //        modelBuilder.Entity<Session>().HasData(
            //        new Session { Id = 1, UserId = 1, Notes = "Initial Test Session", LastUpdated = DateTime.Now,}
            //);
            //        // initial tokens for the session.
            //        modelBuilder.Entity<Token>().HasData(
            //        new Token { Id = 1, SessionID = 1, PieceID = 2, Name = "Active Map", X = 0, Y = 0, },
            //        new Token { Id = 2, SessionID = 1, PieceID = 1, Name = "Goblin", X = 50, Y = 5, zIndex = 1, IsVisible = true },
            //        new Token { Id = 3, SessionID = 1, PieceID = 1, Name = "Knight", X = 50, Y = 10, zIndex = 2, IsVisible = true, }
            //);

            // Production Data:

            modelBuilder.Entity<PieceType>().HasData(
            new PieceType { Id = 1, Name = "Player" },
            new PieceType { Id = 2, Name = "Map" },
            new PieceType { Id = 3, Name = "Structure" },
            new PieceType { Id = 4, Name = "Object" },
            new PieceType { Id = 5, Name = "Goblin" },
            new PieceType { Id = 6, Name = "Orc" },
            new PieceType { Id = 7, Name = "Shop" }
        );

            //user
            modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "Tristan", CreatedAt = DateTime.Now, Email = "tjackson@students.ntc.edu", }
    );

            // sets
            modelBuilder.Entity<Set>().HasData(
            new Set { Id = 1, Name = "Base Set", Price = 0.00m, },
            new Set { Id = 2, Name = "Expansion 1", Price = 4.99m, }
    );

            // pieces
            modelBuilder.Entity<Piece>().HasData(
            new Piece { Id = 1, PieceTypeID = 1, SetID = 1, Name = "Cleric", ImagePath = "/images/Cleric.png", Price = 0.00m },
            new Piece { Id = 2, PieceTypeID = 2, SetID = 1, Name = "Default Dungeon", ImagePath = "/images/testMap.png", Price = 0.00m },
            new Piece { Id = 3, PieceTypeID = 1, SetID = 1, Name = "Goblin Chief", ImagePath = "/images/GoblinChief.png", Price = 0.00m },
            new Piece { Id = 4, PieceTypeID = 1, SetID = 1, Name = "Basic Chest", ImagePath = "/images/chest.png", Price = 0.00m }
    );

            // Session
            modelBuilder.Entity<Session>().HasData(
            new Session { Id = 1, UserId = 1, Notes = "Production Test Session", LastUpdated = DateTime.Now, }
    );
            // initial tokens for the test session.
            modelBuilder.Entity<Token>().HasData(
            new Token { Id = 4, SessionID = 1, PieceID = 1, Name = "Cleric", X = 50, Y = 15, zIndex = 3, IsVisible = true, },
            new Token { Id = 5, SessionID = 1, PieceID = 1, Name = "Cleric", X = 50, Y = 20, zIndex = 4, IsVisible = true, },

            new Token { Id = 1, SessionID = 1, PieceID = 2, Name = "Default Dungeon", X = 0, Y = 0, },
            new Token { Id = 2, SessionID = 1, PieceID = 3, Name = "Goblin Chief", X = 50, Y = 5, zIndex = 1, IsVisible = true },
            new Token { Id = 3, SessionID = 1, PieceID = 4, Name = "Basic Chest", X = 50, Y = 10, zIndex = 2, IsVisible = false, }
    );
        }
    }
}
