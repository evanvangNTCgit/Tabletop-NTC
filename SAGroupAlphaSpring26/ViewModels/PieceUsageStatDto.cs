using System.ComponentModel;

namespace SAGroupAlphaSpring26.ViewModels
{
    public class PieceUsageStatDto
    {
        public int PieceId { get; set; }
        public string PieceName { get; set; } = "";
        public string ImagePath { get; set; } = "/images/default.png";
        public string PieceTypeName { get; set; } = "";
        public int TotalUsed { get; set; }
    }
}
