using Microsoft.AspNetCore.Mvc.Rendering;
using depi__project.viewmodels.prescriptionitems;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.prescription
{
    public class AddPrescriptionVM
    {
        [Required(ErrorMessage = "Prescription date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Prescription Date")]
        public DateTime prescriptiondate { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        public string? notes { get; set; }

        [Required(ErrorMessage = "Visit is required")]
        [Display(Name = "Visit")]
        public int visitid { get; set; }

        [Display(Name = "Patient")]
        public string? PatientName { get; set; }

        [MinLength(1, ErrorMessage = "At least one prescription item is required")]
        public List<AddPrescriptionItemVM> items { get; set; } = new List<AddPrescriptionItemVM>();

        public ICollection<SelectListItem> Visits { get; set; } = new List<SelectListItem>();
    }
}
