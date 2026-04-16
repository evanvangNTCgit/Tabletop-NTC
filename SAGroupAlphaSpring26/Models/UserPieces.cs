namespace SAGroupAlphaSpring26.Models;

public class UserPieces
{
    // ID of user that owns piece.
    public int UserId { get; set; }

    public User User { get; set; } = null!; // added null to get rid of warning

    // ID of piece User owns.
    public int PieceId { get; set; }

    public Piece Piece { get; set; } = null!; // added null to get rid of warning
}
