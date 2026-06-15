using depi__project.enums;

namespace depi__project.viewmodels.userviewmodel
{
    public class responseuserviewmodel
    {
        public string address { get; set; }
        public DateTime createdat { get; set; }
        public DateTime updatedat { get; set; }
        public bool IsActive { get; set; }
        public Gender Gender { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
     //   public List<string> ErrorMessage { get; set; } = new List<string>();
       // public List<string> SuccessMessage { get; set; } = new List<string>();
    }
}
