using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class resptionist
    {
        [Key]
        public int resptionistid { get; set; }
        public decimal salary {  get; set; }
        public userstatus status { get; set; }
        public DateTime hiredate { get; set; }
         //navigation properity
        public string userid { get; set; }
        [ForeignKey(nameof(userid))]
        public Aplicationuser user { get; set; }
        public ICollection<Appoinment> appoinments { get; set; }=new List<Appoinment>();
    }
}
