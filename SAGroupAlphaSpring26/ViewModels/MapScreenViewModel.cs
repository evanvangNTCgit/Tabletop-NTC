namespace SAGroupAlphaSpring26.ViewModels
{
    public class MapScreenViewModel
    {
        // current session.
        public Session? CurrentSession { get; set; }

        // image path for current map, defaults to default image.
        public string MapImagePath { get; set; } = "/images/testMap.png";

        // list of tokens to be displayed.
        public List<Token> Tokens { get; set; } = new List<Token>();

        // List of Pieces for the collection menu.
        public List<Piece> PlayablePieces { get; set; } = new List<Piece>();
    }
}
