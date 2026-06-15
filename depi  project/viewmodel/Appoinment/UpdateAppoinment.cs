using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.Appoinment
{
    public class UpdateAppoinment
    {
        [Required(ErrorMessage = "appoinment id is required")]
        public int AppoinmentId { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Appoinmentdate { get; set; }

        [Required(ErrorMessage = "Appointment start time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime startat { get; set; }

        [Required(ErrorMessage = "Appointment end time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime endat { get; set; }

        [Required(ErrorMessage = "cost is required")]
        [Range(1, 10000, ErrorMessage = "cost must be greater than 0")]
        public decimal cost { get; set; }

        [Required(ErrorMessage = "type is required")]
        [EnumDataType(typeof(typeofappoinment), ErrorMessage = "invalid appointment type")]
        public typeofappoinment type { get; set; }
        public int resptionistidid { get; set; }
        public string? notes { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        public int doctorid { get; set; }
    }
}
