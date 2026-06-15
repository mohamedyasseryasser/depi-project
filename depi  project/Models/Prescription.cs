using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Prescription
    {
        [Key]
        public int prescriptionid { get; set; }
        public DateTime prescriptiondate {  get; set; }
        public string notes {  get; set; }
        //navigation properity
        public ICollection<Prescriptionitems> items { get; set; }  = new List<Prescriptionitems>();
        public int visitid {  get; set; }
        [ForeignKey(nameof(visitid))]
        public Visit? Visit {  get; set; }
    }
}
