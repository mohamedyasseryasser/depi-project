using depi__project.viewmodels.Appoinment;

namespace depi__project.viewmodels.home
{
    public class dashboardvm
    {
        public int patientcount {  get; set; }
        public int appoinmentcount {  get; set; }
        public int totaldoctors {  get; set; }
        public decimal revenue {  get; set; }
        public List<ResponseAppoimentVM> items { get; set; }=new List<ResponseAppoimentVM>();
    }
}
