using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.Appoinment
{
    public class SearchAppoinmentVM
    {

         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Appoinmentdate { get; set; }=DateTime.Now;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
    ErrorMessage = "Phone number format is invalid")]
        public string PhoneNumber { get; set; }

         [EnumDataType(typeof(typeofappoinment), ErrorMessage = "invalid appointment type")]
        public typeofappoinment? type { get; set; }
        public AppointmentStatus? state {  get; set; }
    }
}
