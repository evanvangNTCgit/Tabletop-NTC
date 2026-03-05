namespace SAGroupAlphaSpring26.Models
{
    public class Token
    {
        // Id of the token.
        public int TokenId { get; set; }

        // Name of token.
        public string Name { get; set; }

        // The ID of the piece this token is based on.
        public int PieceID { get; set; }

        // Navigation property to the piece this token is based on.
        public required Piece Piece { get; set; }

        // The current session this token is in.
        public int SessionId { get; set; }

        // The navigation property to the session this token is in.
        public Session? Session { get; set; }

        // Current X coordinate of the token on the map, defaults to 0.00.
        public decimal X { get; set; } = 0.00m;

        // Current Y coordinate of the token on the map, defaults to 0.00.
        public decimal Y { get; set; }

        // The layer that the token is on. Defaults to 0, so bottom layer.
        public int ZIndex { get; set; } = 0;

        // Is this token visible or not, by default yes.
        public bool Visibility { get; set; } = true;
    }
}
