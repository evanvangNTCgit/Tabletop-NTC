namespace SAGroupAlphaSpring26.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        public int PieceId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public User User { get; set; } = null!;

        public Piece Piece { get; set; } = null!;
    }
}
