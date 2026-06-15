using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class resceptionistviewmodel:userviewmodel
    {
        [Required]
        public decimal salary { get; set; }
        public DateTime hiredate { get; set; }= DateTime.Now;
     }
}
