using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.prescriptionitems
{
    public class UpdatePrescriptionitemvm
    {
        public int? prescriptionitemid { get; set; }

        [Required(ErrorMessage = "Medicine is required")]
        [Display(Name = "Medicine")]
        public int mdeicineid { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(100)]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequency is required")]
        [StringLength(100)]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [StringLength(100)]
        [Display(Name = "Duration")]
        public string Duration { get; set; } = string.Empty;

        [Required(ErrorMessage = "Instructions are required")]
        [StringLength(500)]
        [Display(Name = "Instructions")]
        public string Instructions { get; set; } = string.Empty;

        public int? prescriptionid { get; set; }

        public ICollection<SelectListItem> medicines { get; set; } = new List<SelectListItem>();
    }
}
