namespace SAGroupAlphaSpring26.Models
{
    public class Set
    {
        // ID for the sets. Used to group pieces for the marketplace and collections.
        public int Id { get; set; }

        // name of set.
        public string Name { get; set; } = string.Empty;

        // Price of the Set.
        public decimal Price { get; set; } = 0.00m;


        // Collection of pieces in the set.
        public virtual ICollection<Piece> Pieces { get; set; }
    }
}
