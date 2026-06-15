using Microsoft.AspNetCore.Mvc.Rendering;
using depi__project.enums;

namespace depi__project.viewmodels.userviewmodel
{
    public class updatedoctorviewmodel:updateuserviewmodel
    {
        public int DoctorId { get; set; }
        public string Specialization { get; set; }
        public DateTime hiredate { get; set; }
        public decimal salary { get; set; }
         public int DepartmentId { get; set; }
        public ICollection<SelectListItem> deptids { get; set; } = new List<SelectListItem>();

    }
}
