using depi__project.viewmodels.prescriptionitems;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.prescription
{
    public class ResponsePrescriptionVM
    {
        public int prescriptionid { get; set; }

        [Display(Name = "Prescription Date")]
        public DateTime prescriptiondate { get; set; }

        public string notes { get; set; } = string.Empty;
        public int visitid { get; set; }
        public int doctorid { get; set; }
        public int patientid { get; set; }

        [Display(Name = "Patient")]
        public string patientname { get; set; } = string.Empty;

        public string phonenumber { get; set; } = string.Empty;

        [Display(Name = "Doctor")]
        public string? DoctorName { get; set; }

        public DateTime? VisitDate { get; set; }

        public List<ResponseRescriptionitemVM> prescriptionitems { get; set; } = new List<ResponseRescriptionitemVM>();
    }
}
