using depi__project.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Admin
    {
        [Key]
        public int Adminid { get; set; }
        [MinLength(4)]
        public string permissions { get; set; }
        public userstatus status { get; set; }
        //navigation properity
        public string userid { get; set; }
        [ForeignKey(nameof(userid))]
        public Aplicationuser user { get; set; }
    }
}
