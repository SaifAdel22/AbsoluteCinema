using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels
{
    public class UserProfileVM
    {
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters long.")]
        public string? FirstName { get; set; } = string.Empty;

        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long.")]
        public string? LastName { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Display(Name = "Current Password")]
        [PasswordPropertyText]
        public string? CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [PasswordPropertyText]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [PasswordPropertyText]
        public string? ConfirmNewPassword { get; set; }
    }
}