namespace SAGroupAlphaSpring26.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> Pieces { get; set; } = new();

        public List<CartItemSet> Sets { get; set; } = new();
    }
}
