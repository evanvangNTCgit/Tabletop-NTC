namespace SAGroupAlphaSpring26.Models
{
    public class Token
    {
        // Id of the token.
        public int Id { get; set; }

        // X coordinate of the token on the map, defaults to 0.
        public double X { get; set; }

        // Y coordinate of the token on the map, defaults to 0.
        public double Y { get; set; }

        // used to determine the order/layers for the tokens.
        public int zIndex { get; set; }

        // Name of the token, defaults to piece name, but can be changed by user.
        public string Name { get; set; }

        // sets if the token is visible on the map in the popout view, defaults to true.
        public bool IsVisible { get; set; } = true;

        // session ID for the token, used to link tokens to sessions.
        public int SessionID { get; set; }

        // piece ID for the token, used to link tokens to pieces.
        public int PieceID { get; set; }

        // Uses the same image as the piece that it represents.
        // public Image image { get; set; }

        // Links Session to tokens.
        public virtual Session Session { get; set; }

        // Links Piece to tokens.
        public virtual Piece Piece { get; set; }

        
    }
}
