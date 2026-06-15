using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int FloorNumber { get; set; }
        public string Phone { get; set; }
        public bool isactive { get; set; }
        public DateTime createdat {  get; set; }
        public DateTime? updatedat { get; set; }
        public string description {  get; set; }




        //navigation proberity
        public string userid { get; set; }
        [ForeignKey(nameof(userid))]
        public Aplicationuser user { get; set; }
        public ICollection<Doctor> doctors { get; set; }=new List<Doctor>();
    }
}
