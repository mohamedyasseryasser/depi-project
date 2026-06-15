using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class updateuserviewmodel
    {
        [Required]
        public string userid { get; set; }
        [MinLength(5, ErrorMessage = "adress must by 5 character minemim")]

        public string address { get; set; }
        [Required]
        public Gender? Gender { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256)]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
       ErrorMessage = "Invalid Egyptian phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="update checkbox")]
        public bool IsActive {  get; set; }=true;
      //  public List<string> ErrorMessage { get; set; } = new List<string>();
       // public List<string> SuccessMessage { get; set; } = new List<string>();
    }
}
