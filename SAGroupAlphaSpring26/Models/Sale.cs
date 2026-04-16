using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class Sale
    {
        // The ID Of the sale from user.
        public int Id { get; set; }

        // The ID of the user on the sale.
        [Required]
        public int UserID { get; set; }

        // The navigation property to the user.
        [Required]
        public User User { get; set; }

        // The date of the sale.
        public DateTime Date { get; set; } = DateTime.Now;

        public List<SaleLine> SaleLines { get; set; } = new();

        [NotMapped]
        public decimal Tax
        {
            get
            {
                return Math.Round(SaleLines.Sum(ol => ol.TotalTax), 2);
            }
        }

        [NotMapped]
        public decimal Price
        {
            get
            {
                return Math.Round(SaleLines.Sum(ol => ol.TotalCost), 2);
            }
        }
    }
}
