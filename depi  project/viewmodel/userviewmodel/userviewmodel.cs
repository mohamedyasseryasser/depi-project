using depi__project.enums;
using System.ComponentModel.DataAnnotations;
namespace depi__project.viewmodels.userviewmodel
{
    public class userviewmodel
    {
        [Required]
        [MinLength(5,ErrorMessage ="adress must by 5 character minemim")]
        public string address { get; set; }
        public DateTime createdat { get; set; }= DateTime.UtcNow;
        public DateTime updatedat { get; set; }=DateTime.UtcNow;
        public bool IsActive { get; set; }

        [Required]
        public Gender Gender { get; set; }
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
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
         ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
         ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string confirmpassword { get; set; }
    //    public List<string> ErrorMessage { get; set; }= new List<string>();
      //  public List<string> SuccessMessage { get; set; } = new List<string>();
    }
}
