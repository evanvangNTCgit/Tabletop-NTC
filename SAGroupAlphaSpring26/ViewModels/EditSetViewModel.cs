namespace SAGroupAlphaSpring26.ViewModels
{
    public class EditSetViewModel
    {
        public Set SetToEdit { get; set; }

        public List<int> SelectedPieceIds { get; set; } = new();
        public List<Piece> PiecesAvailable { get; set; } = new();
    }
}
