namespace SAGroupAlphaSpring26.ViewModels;

// The reasononing for this viewmodel is because when you add pieces
// We have to get a list of piece types for that piece
// Thus the piece Model is not enough.
public class PieceViewModel
{
    public Piece? Piece { get; set; }

    public List<PieceType> PieceTypes { get; set; } = new();

    // The list of current image paths available
    public List<string> ImagePaths { get; set; } = new();

    // Allow the user to add a custom image if they desire to.
    // Nullable since if they dont wish to just use our aleady used images.
    public IFormFile? UserImageUpload { get; set; }
}
