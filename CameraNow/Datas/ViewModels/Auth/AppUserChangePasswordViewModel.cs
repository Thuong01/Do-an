using System.ComponentModel.DataAnnotations;

namespace Datas.ViewModels.Auth
{
    public class AppUserChangePasswordViewModel
    {
        public AppUserChangePasswordViewModel()
        {
        }

        public AppUserChangePasswordViewModel(string currentPassword, string newPassword, string confirmPassword)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
