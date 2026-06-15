using Microsoft.AspNetCore.Identity;
using depi__project.enums;
using System.ComponentModel.DataAnnotations;

namespace depi__project.Models
{
    public class Aplicationuser:IdentityUser
    {
        [MinLength(5)]
        public string address { get; set; }
        public DateTime createdat { get; set; } = DateTime.UtcNow;
        public DateTime updatedat { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }
    }
}
