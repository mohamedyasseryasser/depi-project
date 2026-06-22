using depi__project.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.invoice
{
    public class AddInvoiceVM
    {
        [Required(ErrorMessage = "Visit is required")]
        [Display(Name = "Visit")]
        public int VisitId { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Total amount must be greater than zero")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Discount cannot be negative")]
        [Display(Name = "Discount")]
        public decimal Discount { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Tax cannot be negative")]
        [Display(Name = "Tax")]
        public decimal Tax { get; set; }

        [Display(Name = "Final Amount")]
        public decimal FinalAmount { get; set; }

        [Required]
        [Display(Name = "Status")]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

        [Display(Name = "Patient")]
        public string? PatientName { get; set; }

        public ICollection<SelectListItem> Visits { get; set; } = new List<SelectListItem>();
    }
}
