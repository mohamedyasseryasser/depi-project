using depi__project.enums;
namespace depi__project.viewmodels.Patient
{
    public class ResponsePatientVM
    {
        public int patientid { get; set; }
        public string patientname { get; set; }
        public string phonenumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime datebirth { get; set; }
        public long? nationalid { get; set; }
        public bool isvalid { get; set; }

        // Nested ViewModels for detailed responses
        public List<AppointmentResponseVM> Appointments { get; set; } = new List<AppointmentResponseVM>();
        public List<VisitResponseVM> Visits { get; set; } = new List<VisitResponseVM>();
        public List<PrescriptionResponseVM> Prescriptions { get; set; } = new List<PrescriptionResponseVM>();
        public List<InvoiceResponseVM> Invoices { get; set; } = new List<InvoiceResponseVM>();
    }

    public class AppointmentResponseVM
    {
        public int appoimentid { get; set; }
        public DateTime Appoinmentdate { get; set; }
        public DateTime startat { get; set; }
        public DateTime endat { get; set; }
        public string notes { get; set; }
        public AppointmentStatus status { get; set; }
        public string DoctorName { get; set; }
    }

    public class VisitResponseVM
    {
        public int visitid { get; set; }
        public DateTime visitdate { get; set; }
        public string diagnosis { get; set; }
        public string notes { get; set; }
        public VisitStatus visitstatus { get; set; }
    }

    public class PrescriptionResponseVM
    {
        public int prescriptionid { get; set; }
        public DateTime prescriptiondate { get; set; }
        public string notes { get; set; }
        public List<string> Medicines { get; set; } = new List<string>();
    }

    public class InvoiceResponseVM
    {
        public int InvoiceId { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
