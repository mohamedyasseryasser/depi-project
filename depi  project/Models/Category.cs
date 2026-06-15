using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Category
    {
        [Key]
        public int cat_id {  get; set; }
        public string cat_name { get; set; }
        public string cat_description { get; set; }
        public bool isactive {  get; set; }
        //navigation proberity
        public string? AddedBy { get; set; }
        public string? user_id {  get; set; }    
        [ForeignKey(nameof(user_id))]
        public Aplicationuser? user { get; set; }
        public ICollection<Medicine> medicines { get; set; }=new List<Medicine>(); 
    }
}
