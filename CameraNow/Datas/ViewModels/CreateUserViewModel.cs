using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels
{
    public class CreateUserViewModel
    {
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime? Birthday { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "EmailConfirmed")]
        public bool EmailConfirmed { get; set; } = false;

        [Phone]
        [Required]
        [Display(Name = "PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "PhoneNumberComfirmed")]
        public bool PhoneNumberComfirmed { get; set; } = false;
    }
}
