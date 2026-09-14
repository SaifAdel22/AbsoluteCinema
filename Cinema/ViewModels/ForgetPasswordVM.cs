using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels;

public class ForgetPasswordVM
{
    [Required]
    [Display(Name = "Email Or UserName")]
    public string EmailOrUserName { get; set; } = string.Empty;
}
