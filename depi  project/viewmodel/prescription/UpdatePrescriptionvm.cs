using depi__project.viewmodels.prescriptionitems;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.prescription
{
    public class UpdatePrescriptionvm
    {
        [Required]
        public int prescriptionid { get; set; }

        [Required(ErrorMessage = "Prescription date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Prescription Date")]
        public DateTime prescriptiondate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        public string notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "Visit is required")]
        [Display(Name = "Visit")]
        public int visitid { get; set; }

        [Display(Name = "Patient")]
        public string? PatientName { get; set; }

        [MinLength(1, ErrorMessage = "At least one prescription item is required")]
        public List<UpdatePrescriptionitemvm> items { get; set; } = new List<UpdatePrescriptionitemvm>();
    }
}
