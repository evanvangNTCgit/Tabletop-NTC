namespace SAGroupAlphaSpring26.ViewModels
{
    public class SetStatsViewModel
    {
        public Set Set { get; set; } = null!;
        public int TotalPurchased { get; set; }

        public decimal PurchasedAmountTotal { get; set; } = 0.00m;
    }
}
