using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.Appoinment
{
    public class UpdateAppoinmentStateVM
    {
        [Required(ErrorMessage = "appoinment id is required")]
        public int AppoinmentId { get; set; }

        [Required(ErrorMessage = "statues is required")]
        public AppointmentStatus Status { get; set; }
        public string patientname {  get; set; }
        public string phone {  get; set; }
    }
}
