namespace SAGroupAlphaSpring26.Models
{
    public class PaginatedSetList
    {
        public List<Set> SetsList { get; set; }

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; private set; }

        public PaginatedSetList(List<Set> sets, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.SetsList = sets;
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
        public static PaginatedSetList Create(List<Set> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedSetList(items, count, pageIndex, pageSize);
        }
    }
}
