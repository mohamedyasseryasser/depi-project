using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Tax { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public InvoiceStatus Status { get; set; } 

        // العلاقة مع Visit
        public int VisitId { get; set; }

        [ForeignKey(nameof(VisitId))]
        public Visit? Visit { get; set; }
    }
}
