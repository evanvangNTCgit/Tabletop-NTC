namespace SAGroupAlphaSpring26.Models
{
    // Represents a piece in the stores and collection. This is a "Template" used to create Tokens for the Map. Stored in Marketplace.
    public class Piece
    {
        // Piece ID.
        public int Id { get; set; }

        // Name of the piece.
        public string Name { get; set; } = "Default Name";

        // Description of the piece.
        public string Description { get; set; } = "Default Description";

        // Default price is set to 0.00, for now.
        public decimal Price { get; set; } = 0.00m;

        // Type ID for a piece, used to link pieces to piece types.
        public int PieceTypeID { get; set; }

        // Type of a piece, examples: "Player", "Skeleton", "Goblin", "Map", "Structure", etc.
        public virtual PieceType PieceType { get; set; }

        // SetID for a piece, used to link pieces to sets.
        public int SetID { get; set; }

        // used to get the Set.Name for the piece.
        public virtual Set Set { get; set; }

        // gets image ID to access the image path. 
        public string ImagePath { get; set; } = "/images/default.png";
    }
}
