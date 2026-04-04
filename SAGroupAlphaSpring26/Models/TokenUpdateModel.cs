namespace SAGroupAlphaSpring26.Models
{
    // Helper class for token JSON data.
    public class TokenUpdateModel
    {
        public required string Id { get; set; } // added "required" to get rid of warning
        public double X { get; set; }
        public double Y { get; set; }
        public int zIndex { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty; // added "= string.Empty" to get rid of warning
        public int PieceId { get; set; }
        public bool Visibility { get; set; }
    }
}
