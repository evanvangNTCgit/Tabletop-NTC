namespace SAGroupAlphaSpring26.Models
{
    public class SaleLine
    {
        // The ID of the saleline. 
        public required int SaleLineID { get; set; }

        // Since a saleline can be apart of the same transaction (so a set of 10 skeletons was bought), we need to link salelines to a single Sale. 
        public int SaleID { get; set; }

        public required Sale Sale {get; set; }

        // The ID of the piece sold in this line.
        public int PieceID { get; set; }

        // The navigation property to the piece sold in this line.
        public required Piece Piece { get; set; }

        // The price of the sale line.
        public decimal Price { get; set; }
    }
}
