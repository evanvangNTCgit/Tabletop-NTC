using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class Set
    {
        // ID for the sets. Used to group pieces for the marketplace and collections.
        public int Id { get; set; }

        // name of set.
        public string Name { get; set; } = string.Empty;

        // Price of the Set.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0.00m;

        // Many to many for pieces and sets since a piece can be in many sets, and a set can have many pieces. We will need to create a join table for this relationship.
        public List<Piece> PiecesList { get; set; } = new List<Piece>();
    }
}
