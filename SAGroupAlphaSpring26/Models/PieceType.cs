namespace SAGroupAlphaSpring26.Models
{
    // Type of a piece, examples: "Player", "Map", "Structure", "Goblin", etc.
    public class PieceType
    {
        // id for piecetype
        public int Id { get; set; }
        // name of piecetype
        public string Name { get; set; } = string.Empty;
    }
}
