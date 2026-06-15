using depi__project.Models;
using depi__project.viewmodels.prescriptionitems;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.viewmodels.prescription
{
    public class UpdatePrescriptionvm
    {
        public int prescriptionid { get; set; }
        public DateTime prescriptiondate { get; set; }
        public string notes { get; set; }
        public int? visitid { get; set; }

        public List<UpdatePrescriptionitemvm> items { get; set; } = new List<UpdatePrescriptionitemvm>();
    }
}
