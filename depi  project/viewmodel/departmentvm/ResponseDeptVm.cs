using depi__project.viewmodels.userviewmodel;

namespace depi__project.viewmodels.departmentvm
{
    public class ResponseDeptVm
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int FloorNumber { get; set; }
        public string Phone { get; set; }
        public bool isactive { get; set; }
        public DateTime createdat { get; set; }
        public DateTime updatedat { get; set; }
        public string description { get; set; }
        public string userid { get; set; }
        public string UserName { get; set; }
        public int DoctorCount { get; set; }
        public ICollection<doctorresponseviewmodel> doctors { get; set; } = new List<doctorresponseviewmodel>();
    }
}
