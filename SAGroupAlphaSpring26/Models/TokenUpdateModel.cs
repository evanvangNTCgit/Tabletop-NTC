namespace SAGroupAlphaSpring26.Models
{
    // Helper class for token JSON data.
    public class TokenUpdateModel
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int zIndex { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
