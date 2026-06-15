using Microsoft.AspNetCore.Mvc.Rendering;

namespace depi__project.viewmodels.category
{
    public class UpdateCategoryVM
    {
        public int cat_id { get; set; }
        public string cat_name { get; set; }
        public string cat_description { get; set; }
        public string user_id { get; set; }
        public bool isactive { get; set; }
        public ICollection<SelectListItem> admins {  get; set; }=new List<SelectListItem>();
    }
}
