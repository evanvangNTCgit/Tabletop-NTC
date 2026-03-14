using Microsoft.AspNetCore.Identity;
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
        public DbSet<Token> Tokens { get; set; }

        // So this is a reference to a PieceTypes table.
        public DbSet<PieceType> PieceTypes { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<PieceSets> PieceSets { get; set; }

        public DbSet<SaleLine> SaleLines { get; set; }
        public DbSet<Sale> Sales { get; set; }

        // We overide the on model creation method
        // BECAUSE HERE... we specify what relationships we want to have in our database, and how we want to structure our tables.
        // Like Pieces and piecetypes have a relationship, so we would specify that here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring some relationships here:

            // As of now 3/5/26
            // When it comes to many to many I have lists put in both models
            // So the model builder can handle this connection for us.

            // One piece type can have many pieces, but a piece ONLY one type.
            modelBuilder.Entity<PieceType>()
                .HasMany(pt => pt.Pieces)
                .WithOne(p => p.PieceType)
                .HasForeignKey(p => p.PieceTypeID);

            // A user can have many sessions...
            modelBuilder.Entity<User>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            // A sale can have many sale Lines, but a sale line can only have one sale.
            // Review sale.salelines property for further understanding.
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleLines)
                .WithOne(sl => sl.Sale)
                .HasForeignKey(sl => sl.SaleID);

            // Initial piece types
            modelBuilder.Entity<PieceType>().HasData(
                                new PieceType { Id = 1, Name = "Player" },
                new PieceType { Id = 2, Name = "Map" },
                new PieceType { Id = 3, Name = "Structure" },
                new PieceType { Id = 4, Name = "Object" },
                new PieceType { Id = 5, Name = "Goblin" },
                new PieceType { Id = 6, Name = "Orc" },
                new PieceType { Id = 7, Name = "Shop" }
                );

            // Initial Piece
            modelBuilder.Entity<Piece>().HasData(
                                new Piece { Id = 1, PieceTypeID = 2, Name = "Default Dungeon", ImagePath = "/images/testMap.png", Price = 0.00m },
                new Piece { Id = 2, PieceTypeID = 1, Name = "Cleric", ImagePath = "/images/Cleric.png", Price = 0.00m },
                new Piece { Id = 3, PieceTypeID = 5, Name = "Goblin Chief", ImagePath = "/images/GoblinChief.png", Price = 0.00m },
                new Piece { Id = 4, PieceTypeID = 4, Name = "Basic Chest", ImagePath = "/images/chest.png", Price = 0.00m },
                new Piece { Id = 5, PieceTypeID = 1, Name = "Bard", ImagePath = "/images/bardTest.png", Price = 0.00m }
                );

            // A password hasher 
            PasswordHasher<string> passwordHasher = new();
            // Seed User
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Local", LastName = "DM", PasswordHash = passwordHasher.HashPassword(null!, "Password123"), Email = "local@demo.com", IsAdmin = true },
                new User { Id = 2, FirstName = "Evan", LastName = "Vang", PasswordHash = passwordHasher.HashPassword(null!, "EvanPassword123"), Email = "evankvang@gmail.com", IsAdmin = false }
                );

            // A seed session
            modelBuilder.Entity<Session>().HasData(
                new Session { Id = 1, UserId = 1, Name = "Test Session", Notes = "Local Test Session", LastUpdated = DateTime.Now, },
                new Session { Id = 2, UserId = 1, Name = "Test Session 2", Notes = "Local Test Session 2", LastUpdated = DateTime.Now, });

            modelBuilder.Entity<Token>().HasData(
                // Session 1 Token Seed Data.
                new Token { Id = 1, SessionId = 1, PieceID = 1, Name = "Default Dungeon", X = 0, Y = 0, ZIndex = 0, Visibility = true },
                new Token { Id = 2, SessionId = 1, PieceID = 2, Name = "Cleric", X = 50, Y = 15, ZIndex = 3, Visibility = true },
                new Token { Id = 3, SessionId = 1, PieceID = 3, Name = "Goblin Chief", X = 50, Y = 5, ZIndex = 1, Visibility = true },
                new Token { Id = 4, SessionId = 1, PieceID = 4, Name = "Basic Chest", X = 50, Y = 10, ZIndex = 2, Visibility = false },

                // Token Seed Data for 2nd Session testing.
                new Token { Id = 5, SessionId = 2, PieceID = 1, Name = "Default Dungeon", X = 0, Y = 0, ZIndex = 0, Visibility = true },
                new Token { Id = 6, SessionId = 2, PieceID = 2, Name = "Cleric", X = 50, Y = 15, ZIndex = 3, Visibility = true },
                new Token { Id = 7, SessionId = 2, PieceID = 2, Name = "Cleric", X = 50, Y = 5, ZIndex = 1, Visibility = true },
                new Token { Id = 8, SessionId = 2, PieceID = 2, Name = "Cleric", X = 50, Y = 10, ZIndex = 2, Visibility = true },
                new Token { Id = 9, SessionId = 2, PieceID = 3, Name = "Goblin Chief", X = 50, Y = 5, ZIndex = 1, Visibility = true });

            // Making a composite key for the UserPieces.
            modelBuilder.Entity<UserPieces>()
                .HasKey(p => new { p.PieceId, p.UserId }); // A composite key of piece ID and User ID.

            modelBuilder.Entity<UserPieces>()
                .HasOne(up => up.Piece) // Each UserPiece has one Piece
                .WithMany(p => p.Owners) // Each piece has many owners
                .HasForeignKey(p => p.PieceId); // Each UserPiece has a foreign key to the piece ID.

            modelBuilder.Entity<UserPieces>()
                .HasOne(up => up.User) // Each UserPiece has one user
                .WithMany(u => u.OwnedPieces) // Each user has many owned pieces
                .HasForeignKey(up => up.UserId); // Each user piece has the foreign key to user ID.

            // Composite key for the PieceSets.
            modelBuilder.Entity<PieceSets>()
                .HasKey(p => new { p.PieceId, p.SetId });

            modelBuilder.Entity<PieceSets>()
                .HasOne(ps => ps.Set) // Each Piece set has one set.
                .WithMany(s => s.PiecesList) // Each set has many pieces
                .HasForeignKey(ps => ps.SetId); // Each piece set has the foreign key to set ID.

            modelBuilder.Entity<PieceSets>()
                .HasOne(ps => ps.Piece) // Each piece set has one piece.
                .WithMany(p => p.Sets) // Each piece has many sets.
                .HasForeignKey(ps => ps.PieceId); // Each piece set has the foreign key to piece id.

            // Making one piece set:
            modelBuilder.Entity<Set>().HasData(
                new Set { Id=1, Name = "Evans Beginner Pack", Price = 0.00m} // This is a free set for the user c:
                );
            // Putting some pieces in my set.
            modelBuilder.Entity<PieceSets>().HasData(
                new PieceSets { PieceId = 2, SetId = 1, Piece = null!, Set = null! }, // Cleric
                new PieceSets { PieceId = 5, SetId = 1, Piece = null!, Set = null! } // Bard
                );


            // Lets have the local dm own all the seed pieces as of right now.
            // Currently 5 pieces provided in seed data.
            modelBuilder.Entity<UserPieces>().HasData(
                new UserPieces { UserId = 1, PieceId = 1, Piece = null!, User = null! },
                new UserPieces { UserId = 1, PieceId = 2, Piece = null!, User = null! },
                new UserPieces { UserId = 1, PieceId = 3, Piece = null!, User = null! },
                new UserPieces { UserId = 1, PieceId = 4, Piece = null!, User = null! },
                new UserPieces { UserId = 1, PieceId = 5, Piece = null!, User = null! }
                );

            // Test Data:


            //        // Production Data:
            //        modelBuilder.Entity<PieceType>().HasData(
            //        new PieceType { Id = 1, Name = "Player" },
            //        new PieceType { Id = 2, Name = "Map" },
            //        new PieceType { Id = 3, Name = "Structure" },
            //        new PieceType { Id = 4, Name = "Object" },
            //        new PieceType { Id = 5, Name = "Goblin" },
            //        new PieceType { Id = 6, Name = "Orc" },
            //        new PieceType { Id = 7, Name = "Shop" }
            //    );

            //        //user
            //        modelBuilder.Entity<User>().HasData(
            //        new User { Id = 1, Username = "Tristan", CreatedAt = DateTime.Now, Email = "tjackson@students.ntc.edu", }
            //);

            //        // sets
            //        modelBuilder.Entity<Set>().HasData(
            //        new Set { Id = 1, Name = "Base Set", Price = 0.00m, },
            //        new Set { Id = 2, Name = "Expansion 1", Price = 4.99m, }
            //);

            //        // pieces
            //        modelBuilder.Entity<Piece>().HasData(
            //        new Piece { Id = 1, PieceTypeID = 1, SetID = 1, Name = "Cleric", ImagePath = "/images/Cleric.png", Price = 0.00m },
            //        new Piece { Id = 2, PieceTypeID = 2, SetID = 1, Name = "Default Dungeon", ImagePath = "/images/testMap.png", Price = 0.00m },
            //        new Piece { Id = 3, PieceTypeID = 1, SetID = 1, Name = "Goblin Chief", ImagePath = "/images/GoblinChief.png", Price = 0.00m },
            //        new Piece { Id = 4, PieceTypeID = 1, SetID = 1, Name = "Basic Chest", ImagePath = "/images/chest.png", Price = 0.00m }
            //);

            //        // Session
            //        modelBuilder.Entity<Session>().HasData(
            //        new Session { Id = 1, UserId = 1, Notes = "Production Test Session", LastUpdated = DateTime.Now, }
            //);
            //        // initial tokens for the test session.
            //        modelBuilder.Entity<Token>().HasData(
            //        new Token { Id = 4, SessionID = 1, PieceID = 1, Name = "Cleric", X = 50, Y = 15, zIndex = 3, IsVisible = true, },
            //        new Token { Id = 5, SessionID = 1, PieceID = 1, Name = "Cleric", X = 50, Y = 20, zIndex = 4, IsVisible = true, },

            //        new Token { Id = 1, SessionID = 1, PieceID = 2, Name = "Default Dungeon", X = 0, Y = 0, },
            //        new Token { Id = 2, SessionID = 1, PieceID = 3, Name = "Goblin Chief", X = 50, Y = 5, zIndex = 1, IsVisible = true },
            //        new Token { Id = 3, SessionID = 1, PieceID = 4, Name = "Basic Chest", X = 50, Y = 10, zIndex = 2, IsVisible = false, }
            //);
        }
    }
}
