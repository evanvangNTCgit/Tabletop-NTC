namespace SAGroupAlphaSpring26.Models
{
    // Helper class for token JSON data.
    public class TokenUpdateModel
    {
        public string Id { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public int zIndex { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PieceId { get; set; }
        public bool Visibility { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
