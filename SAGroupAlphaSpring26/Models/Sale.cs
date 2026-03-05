namespace SAGroupAlphaSpring26.Models
{
    public class Sale
    {
        // The ID Of the sale from user.
        public int SaleID { get; set; }

        // The ID of the user on the sale.
        public int UserID { get; set; }

        // The navigation property to the user.
        public required User User { get; set; }

        // The date of the sale.
        public DateTime Date { get; set; } = DateTime.Now;

        // The total price of the sale... maybe just a LINQ QUERY somewhere down the line?
        // Nothing right now just think of this later down the line.
    }
}
