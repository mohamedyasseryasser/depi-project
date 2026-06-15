using depi__project.Models;
using depi__project.viewmodels.prescriptionitems;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.viewmodels.prescription
{
    public class ResponsePrescriptionVM
    {
        public int prescriptionid { get; set; }
        public DateTime prescriptiondate { get; set; }
        public string notes { get; set; }
        public int visitid { get; set; }
        public int doctorid { get; set; }
        public int patientid { get; set; }
        public string patientname { get; set; }
        public string phonenumber { get; set; }
        public List<ResponseRescriptionitemVM> prescriptionitems { get; set; } = new List<ResponseRescriptionitemVM>();
    }
}
