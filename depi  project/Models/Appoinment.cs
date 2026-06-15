using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Appoinment
    {
        [Key]
        public int appoimentid {  get; set; }
        public string PhoneNumber {  get; set; }
        public DateTime Appoinmentdate { get; set; }
        public DateTime startat { get; set; }
        public DateTime endat { get; set; }
        public DateTime? updateat {  get; set; }
        public string? notes {  get; set; }
        public decimal cost {  get; set; }
        public typeofappoinment type {  get; set; }
        public AppointmentStatus status { get; set; } = AppointmentStatus.Pending;
        //navigation properity
        public int doctorid {  get; set; }
        [ForeignKey(nameof(doctorid))]
        public Doctor Doctor { get; set; }
        public int resptionistidid { get; set; }
        [ForeignKey(nameof(resptionistidid))]
        public resptionist resptionist { get; set; }
        public int patientid { get; set; }
        [ForeignKey(nameof(patientid))]
        public Patient Patient { get; set; }
        public Visit? Visit { get; set; }
    }
}
