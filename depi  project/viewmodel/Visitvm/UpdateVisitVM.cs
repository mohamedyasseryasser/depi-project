using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.viewmodels.VisitRepo
{
    public class UpdateVisitVM
    {
        [Required(ErrorMessage = "visit id is required")]
        public int visitid { get; set; }
        public string? notes { get; set; }

        public string? diagnosis { get; set; }

        [Required(ErrorMessage = "statues is required.")]
        public VisitStatus visitstatus { get; set; }
    }
}
