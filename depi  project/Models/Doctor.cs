using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        public string Specialization { get; set; }     
        public DateTime hiredate {  get; set; }
        public decimal salary {  get; set; }
        public userstatus status { get; set; }

        //navigation properity
        public string userid {  get; set; }
        [ForeignKey(nameof(userid))]
        public Aplicationuser user { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
        public ICollection<Appoinment> appoinments { get; set; } = new List<Appoinment>();
    }
}
