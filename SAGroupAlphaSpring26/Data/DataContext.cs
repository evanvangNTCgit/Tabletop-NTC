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

        // So this is a reference to a PieceTypes table.
        /*
        * property in your DbContext class generally corresponds to a table in the database, where TEntity is the C# class (model) that maps to the table's structure.
        */
        public DbSet<PieceType> PieceTypes { get; set; }

        // We overide the on model creation method
        // BECAUSE HERE... we specify what relationships we want to have in our database, and how we want to structure our tables.
        // Like Pieces and piecetypes have a relationship, so we would specify that here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // I will make some test piece types then comment it out...
            //modelBuilder.Entity<PieceType>().HasData(
            //    new PieceType { Id = 1, Name = "Enemy" },
            //    new PieceType { Id = 2, Name = "Map" }
            //);
        }
    }
}
