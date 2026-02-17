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
    }
}
