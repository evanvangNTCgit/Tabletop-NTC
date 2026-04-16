using System.Net.Http.Headers;

namespace SAGroupAlphaSpring26.Models;

public class PieceSets
{
    // ID of piece in set.
    public int PieceId { get; set; }
    public Piece Piece { get; set; } = null!; // added null to get rid of warning

    // Id of the set Piece is in.
    public int SetId { get; set; }

    public Set Set { get; set; } = null!; // added null to get rid of warning
}
