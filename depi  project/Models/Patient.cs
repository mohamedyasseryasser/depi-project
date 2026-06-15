using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Patient
    {
        [Key]
        public int patientid {  get; set; }
        public string patientname {  get; set; }
        public string phonenumber {  get; set; }
        public Gender Gender { get; set; }
        public DateTime datebirth {  get; set; }
        public long? nationalid {  get; set; }
        public bool isvalid { get; set; } = true;
        //navigation properity
        public ICollection<Appoinment> appoinments {  get; set; }=new List<Appoinment>();
    }
}
