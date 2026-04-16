using SAGroupAlphaSpring26.Models;

namespace SAGroupAlphaSpring26.ViewModels
{
    public class PieceUsageStatsViewModel
    {
        public Piece Piece { get; set; } = null!;
        public int TotalUsed { get; set; }
    }
}
