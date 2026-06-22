using depi__project.enums;
using depi__project.viewmodels.Appoinment;

namespace depi__project.viewmodels.Visit
{
    public class ResponseVisitVM
    {
        public int visitid { get; set; }
        public string? notes { get; set; }
        public DateTime visitdate { get; set; }
        public string? diagnosis { get; set; }
        public VisitStatus visitstatus { get; set; }
        public int appoinmentid { get; set; }
        public ResponseAppoimentVM? ResponseAppoimentVM { get; set; }
        public depi__project.Models.Prescription? Prescription { get; set; }
        public depi__project.Models.Invoice? Invoice { get; set; }
    }
}
