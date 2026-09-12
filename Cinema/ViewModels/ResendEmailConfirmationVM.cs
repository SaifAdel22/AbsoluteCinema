using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels
{
    public class ResendEmailConfirmationVM
    {
        [Required]
        [Display(Name = "Email Or UserName")]
        public string EmailOrUserName { get; set; } = string.Empty;
    }

}
