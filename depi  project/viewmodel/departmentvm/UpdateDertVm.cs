using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.departmentvm
{
    public class UpdateDertVm
    {
        [Required]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Floor number is required")]
        [Range(1, 100, ErrorMessage = "Floor number must be between 1 and 100")]
        public int FloorNumber { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        public bool isactive { get; set; } = true;



        [MaxLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string description { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public string userid { get; set; }
    }
}
