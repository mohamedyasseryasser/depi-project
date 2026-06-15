using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class updateadminviewmodel:updateuserviewmodel
    {
        
        [MinLength(3)]
        public string permissions { get; set; }
    }
}
