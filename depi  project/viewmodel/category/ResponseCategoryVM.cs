namespace depi__project.viewmodels.category
{
    public class ResponseCategoryVM
    {
        public int cat_id { get; set; }
        public string cat_name { get; set; }
        public string cat_description { get; set; }
        public string role { get; set; } = "Admin";
        public string? AddedBy {  get; set; }
        public string user_id { get; set; }
        public bool isactive { get; set; }

    }
}
