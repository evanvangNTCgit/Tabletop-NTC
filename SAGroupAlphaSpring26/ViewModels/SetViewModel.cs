using System.ComponentModel.DataAnnotations;
using SAGroupAlphaSpring26.Models;

namespace SAGroupAlphaSpring26.ViewModels
{
    public class SetViewModel
    {
        public Set NewSet { get; set; } = new() { Name = "" };

        public List<Piece> AvailablePieces { get; set; } = new();

        public List<int> SelectedPieceIds { get; set; } = new();
    }
}

