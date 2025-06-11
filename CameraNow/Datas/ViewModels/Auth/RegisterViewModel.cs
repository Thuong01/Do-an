using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
        public string? FullName { set; get; }
        public string? Address { set; get; }
        public string? PhoneNumber { set; get; }

        [Compare("Password", ErrorMessage = "Confirm password does not match!")]
        [Required(ErrorMessage = "Confirm password is required!")]
        public string ConfirmPassword { get; set; }
    }
}
