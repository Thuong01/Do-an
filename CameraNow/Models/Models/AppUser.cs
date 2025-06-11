using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(255)]
        public string? FullName { set; get; }
        [MaxLength(500)]
        public string? Address { set; get; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { set; get; }
        public string Avatar { get; set; } = "/img/avatar_default.jpg";
        public string? PublicId { get; set; }
    }
}
