namespace SAGroupAlphaSpring26.Models
{
    public class Sale
    {
        // The ID Of the sale from user.
        public int SaleID { get; set; }

        // The ID of the user on the sale.
        [Required]
        public int UserID { get; set; }

        // The navigation property to the user.
        public required User User { get; set; }

        // The date of the sale.
        public DateTime Date { get; set; } = DateTime.Now;

        // So a sale has multiple sale lines since for example
        // You buy our set of skeletons (10) so that is 10 sale lines,
        // Then sale can be used to remember that it was under one transaction.
        public List<SaleLine> SaleLines { get; set; } = new();

        // The total price of the sale... maybe just a LINQ QUERY somewhere down the line?
        // Nothing right now just think of this later down the line.
    }
}
