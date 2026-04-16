namespace SAGroupAlphaSpring26.Models
{
    public class CartItemSet
    {
        public int Id { get; set; }

        [Required]
        public int SetId { get; set; } // Id of the set being added to cart.

        [Required]
        public int UserId { get; set; } // If of the user who is adding item to cart.

        [Required]
        public bool IsArchived { get; set; }

        public Set? Set { get; set; }

        public User? User { get; set; }
    }
}
