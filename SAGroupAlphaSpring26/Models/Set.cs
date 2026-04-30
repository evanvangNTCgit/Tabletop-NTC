using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class Set
    {
        // ID for the sets. Used to group pieces for the marketplace and collections.
        public int Id { get; set; }

        // name of set.
        [MinLength(2, ErrorMessage = "Name of set must be at least 2 characters long")]
        public required string Name { get; set; }

        // Price of the Set.
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; } = 0.00m;

        public bool IsArchived { get; set; }

        // Many to many for pieces and sets since a piece can be in many sets, and a set can have many pieces. We will need to create a join table for this relationship.
        public List<PieceSets>? PiecesList { get; set; } = new();

        public List<CartItemSet>? CartItemSets { get; set; } = new();

        public List<SaleLine>? SaleLines { get; set; }
    }
}
