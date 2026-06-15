using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Visit
    {
        [Key]
        public int visitid {  get; set; }
        public string? notes {  get; set; }
        public DateTime visitdate {  get; set; }
        public string? diagnosis {  get; set; }
        public VisitStatus visitstatus { get; set; }
        //navigation proberity
        public int appoinmentid {  get; set; }
        [ForeignKey(nameof(appoinmentid))]
        public Appoinment? Appoinment { get; set; }
        public Prescription? Prescription { get; set; }
        public Invoice? Invoice { get; set; }   
    }
}
