using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string Type { get; set; } = string.Empty;

        public int PieceID { get; set; }

        public int PurchaseID { get; set; }

        public int SetID { get; set; }
    }
}
