using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class SaleLine
    {
        // The ID of the saleline. 
        [Required]
        public int Id { get; set; }

        [Required]
        public int SaleID { get; set; }

        // The ID of the piece sold in this line.
        [Required]
        public int? PieceID { get; set; }

        // SaleLine could be a piece from a set.
        public int? SetID { get; set; }

        // The price of the sale line.
        [Required]
        // What you pay without tax.
        public decimal Price { get; set; }

        [Required]
        // Total tax on sale line.
        public decimal Tax { get; set; }

        [Required]
        // Meaning after you add up the price and the tax.
        public decimal TotalCost { get; set; }

        [NotMapped]
        public decimal TotalTax
        {
            get
            {
                return Math.Round(Tax, 2);
            }
        }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return Math.Round(Price, 2);
            }
        }

        // The navigation property to the piece sold in this line.
        public Piece? Piece { get; set; }

        // Navigation property to set.
        public Set? Set { get; set; }

        // Navigation property to the sale.
        public Sale Sale { get; set; }
    }
}
