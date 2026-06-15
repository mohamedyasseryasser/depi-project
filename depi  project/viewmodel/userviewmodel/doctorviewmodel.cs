using Microsoft.AspNetCore.Mvc.Rendering;
using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class doctorviewmodel:userviewmodel
    {
        [Required]
        [MinLength(4)]
        public string Specialization { get; set; }
         public DateTime hiredate { get; set; }= DateTime.UtcNow;
        [Required]
        public decimal salary { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public ICollection<SelectListItem> deptids { get; set; }= new List<SelectListItem>();
    }
}
