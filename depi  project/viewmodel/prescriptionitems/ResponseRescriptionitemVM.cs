using depi__project.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.viewmodels.prescriptionitems
{
    public class ResponseRescriptionitemVM
    {
        public int prescriptionitemid { get; set; }
        public int quantity { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string notes { get; set; }
         public int prescriptionid { get; set; }
        public int mdeicineid { get; set; }
        public string Name {  get; set; }
    }
}
