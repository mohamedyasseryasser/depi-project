using depi__project.enums;

namespace depi__project.viewmodels.userviewmodel
{
    public class doctorresponseviewmodel:responseuserviewmodel
    {
        public int DoctorId { get; set; }
        public string Specialization { get; set; }
        public DateTime hiredate { get; set; }
        public decimal salary { get; set; }
        public userstatus status { get; set; }
        public int DepartmentId { get; set; }
        public string deptname {  get; set; }
    }
}
