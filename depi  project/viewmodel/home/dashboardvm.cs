using depi__project.viewmodels.Appoinment;

namespace depi__project.viewmodels.home
{
    public class dashboardvm
    {
        public string RoleName { get; set; } = "Staff";
        public string DisplayName { get; set; } = "User";
        public int patientcount { get; set; }
        public int appoinmentcount { get; set; }
        public int totaldoctors { get; set; }
        public int departmentcount { get; set; }
        public int visitcount { get; set; }
        public int medicinecount { get; set; }
        public int lowstockcount { get; set; }
        public int pendinginvoicecount { get; set; }
        public int todayappointmentcount { get; set; }
        public int pendingAppointmentCount { get; set; }
        public int confirmedAppointmentCount { get; set; }
        public int completedAppointmentCount { get; set; }
        public int cancelledAppointmentCount { get; set; }
        public int noShowAppointmentCount { get; set; }
        public int paidInvoiceCount { get; set; }
        public int cancelledInvoiceCount { get; set; }
        public decimal revenue { get; set; }
        public List<string> appointmentTrendLabels { get; set; } = new List<string>();
        public List<int> appointmentTrendCounts { get; set; } = new List<int>();
        public List<ResponseAppoimentVM> items { get; set; } = new List<ResponseAppoimentVM>();
    }
}
