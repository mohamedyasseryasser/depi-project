using depi__project.enums;
using System.ComponentModel.DataAnnotations;
namespace depi__project.viewmodels.Patient
{
    public class AddPatientVM
    {
        [Required(ErrorMessage = "Patient name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string patientname { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string phonenumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime datebirth { get; set; }

        public long? nationalid { get; set; }
    }
}
