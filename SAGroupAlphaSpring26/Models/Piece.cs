using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    // Represents a piece in the stores and collection. This is a "Template" used to create Tokens for the Map. Stored in Marketplace.
    public class Piece
    {
        // Piece ID.
        public int PieceId { get; set; }

        // Name of the piece.
        [MinLength(4, ErrorMessage = "Name of piece must be atleast 5 characters long")]
        [Required]
        public string Name { get; set; } = "Default Name";

        // Description of the piece.        
        public string? Description { get; set; } = "No description provided";

        // Default price is set to 0.00, for now.
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; } = 0.00m;

        // gets image ID to access the image path. 
        public string ImagePath { get; set; } = "/images/default.png";

        // Type ID for a piece, used to link pieces to piece types.
        [Required]
        public int PieceTypeID { get; set; }

        public required PieceType PieceType { get; set; }

        // This is a many to many start since a piece can be in many sets, and a set can have many pieces. We will need to create a join table for this relationship.
        public List<Set> Sets { get; set; } = new();

        // The list of owners that own a this piece. Many to many relationship since a user can own many pieces, and a piece can be owned by many users.
        public List<User> Owners { get; set; } = new();
    }
}
