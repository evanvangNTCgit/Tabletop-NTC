namespace SAGroupAlphaSpring26.Models
{
    // Type of a piece, examples: "Player", "Map", "Structure", "Goblin", etc.
    public class PieceType
    {
        // id for piecetype
        public int PieceTypeId { get; set; }
        // name of piecetype

        [MinLength(4, ErrorMessage = "Name of PieceType must be atleast 4 charcters long")]
        [Required]
        public string Name { get; set; } = string.Empty;

        // The list of pieces a piecetype can have.
        public List<Piece> Pieces { get; set; } = new();
    }
}
