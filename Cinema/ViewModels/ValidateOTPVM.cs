using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels;

public class ValidateOTPVM
{
    [Required]
    public string OTP { get; set; } = string.Empty;
}
