namespace SAGroupAlphaSpring26.ViewModels
{
    public class PaginatedStoreViewModel
    {
        public PaginatedPieceList PaginatedPieces { get; private set; }

        public PaginatedSetList PaginatedSets { get; private set; }

        public PaginatedStoreViewModel(PaginatedSetList PaginatedSets, PaginatedPieceList PaginatedPieces)
        {
            this.PaginatedPieces = PaginatedPieces;
            this.PaginatedSets = PaginatedSets;
        }
    }
}
