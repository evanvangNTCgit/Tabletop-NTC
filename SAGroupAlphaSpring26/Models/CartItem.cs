namespace SAGroupAlphaSpring26.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int PieceId { get; set; } // Id of the piece being added to cart.

        [Required]
        public int UserId { get; set; } // If of the user who is adding item to cart.

        [Required]
        public bool IsArchived { get; set; }

        public Piece? Piece { get; set; }

        public User? User { get; set; }
    }
}
