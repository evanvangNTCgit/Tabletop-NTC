namespace SAGroupAlphaSpring26.Models
{
    // https://www.youtube.com/watch?v=L9VtwtoLvy8
    public class PaginatedPieceList
    {
        public List<Piece> PiecesList { get; set; }

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; private set; }

        public PaginatedPieceList(List<Piece> pieces, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.PiecesList = pieces;
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;

        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalItems);

        /// <summary>
        /// Creates the paginated piece list.
        /// </summary>
        /// <param name="source">Pieces Source</param>
        /// <param name="pageIndex">Current page number to retrieve.</param>
        /// <param name="pageSize">Number of items to display on the page.</param>
        /// <returns>Paginated list of Pieces.</returns>
        public static PaginatedPieceList Create(List<Piece> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedPieceList(items, count, pageIndex, pageSize);
        }
    }
}
