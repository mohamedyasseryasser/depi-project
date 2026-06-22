using depi__project.enums;

namespace depi__project.viewmodels.invoice
{
    public class ResponseInvoiceVM
    {
        public int InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal FinalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? DoctorName { get; set; }
        public int? AppointmentId { get; set; }
    }
}
