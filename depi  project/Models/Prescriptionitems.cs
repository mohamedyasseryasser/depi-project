using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Prescriptionitems
    {
        [Key]
        public int prescriptionitemid {  get; set; }
        public int quantity {  get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string notes {  get; set; }
 
        //navigation proberity
        public int prescriptionid {  get; set; }
        [ForeignKey(nameof(prescriptionid))]
        public Prescription? Prescription { get; set; }
        public int mdeicineid {  get; set; }
        [ForeignKey(nameof(mdeicineid))]
        public Medicine? Medicine { get; set; }

     }
}
