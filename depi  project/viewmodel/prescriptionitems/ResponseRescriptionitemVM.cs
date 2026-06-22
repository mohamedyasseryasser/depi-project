using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.prescriptionitems
{
    public class ResponseRescriptionitemVM
    {
        public int prescriptionitemid { get; set; }
        public int quantity { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        [Display(Name = "Instructions")]
        public string Instructions { get; set; } = string.Empty;

        public int prescriptionid { get; set; }
        public int mdeicineid { get; set; }

        [Display(Name = "Medicine")]
        public string Name { get; set; } = string.Empty;
    }
}
