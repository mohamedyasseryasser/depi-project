using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.userviewmodel
{
    public class responseadminviewmodel:responseuserviewmodel
    {
        public int adminid {  get; set; }
        public string permissions { get; set; }
        public userstatus status { get; set; }  
    }
}
