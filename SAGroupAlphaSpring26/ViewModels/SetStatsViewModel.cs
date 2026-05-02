namespace SAGroupAlphaSpring26.ViewModels
{
    public class SetStatsViewModel
    {
        public Set Set { get; set; } = null!;
        public int TotalPurchased { get; set; }
        private decimal purchaseAmountTotal = 0.00m;

        public decimal PurchasedAmountTotal
        { 
            get
            {
                return Math.Round(this.purchaseAmountTotal, 2);
            }
            set
            {
                this.purchaseAmountTotal = value;
            }
        }
    }
}
