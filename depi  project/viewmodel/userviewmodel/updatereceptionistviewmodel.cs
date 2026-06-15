using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class updatereceptionistviewmodel:updateuserviewmodel
    {
        [Required]
        public int receptionid {  get; set; }
        [Required]
        public decimal salary { get; set; }
        public DateTime hiredate { get; set; } = DateTime.Now;
    }
}
