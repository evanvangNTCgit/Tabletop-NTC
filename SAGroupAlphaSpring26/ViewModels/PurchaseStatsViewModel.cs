using SAGroupAlphaSpring26.Models;

namespace SAGroupAlphaSpring26.ViewModels
{
    public class PurchaseStatsViewModel
    {
        public Piece Piece { get; set; } = null!;
        public int TotalPurchased { get; set; }

        public decimal PurchasedAmountTotal { get; set; } = 0.00m;
    }
}
