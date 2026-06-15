using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class adminviewmodel : userviewmodel
    {
        [Required]
        [MinLength(4,ErrorMessage ="permission must by 4 char minemim")]
        public string permissions { get; set; }
    }
}
