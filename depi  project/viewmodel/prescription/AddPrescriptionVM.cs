using Microsoft.AspNetCore.Mvc.Rendering;
using depi__project.viewmodels.prescriptionitems;

namespace depi__project.viewmodels.prescription
{
    public class AddPrescriptionVM
    {
        public DateTime prescriptiondate { get; set; }= DateTime.Now;
        public string? notes { get; set; }
        public ICollection<AddPrescriptionItemVM> items { get; set; } = new List<AddPrescriptionItemVM>();
        public int? visitid { get; set; }
      

    }
}
