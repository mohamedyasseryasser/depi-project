using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class responsereceptionistviewmodel:responseuserviewmodel
    {
        [Required]
        public int resptionistid { get; set; }
        public decimal salary { get; set; }
        public userstatus status { get; set; }
        public DateTime hiredate { get; set; }
    }
}
