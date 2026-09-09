using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AbsoluteCinema.ViewModels
{
    public class ActorVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Actor name is required.")]
        [Display(Name = "Actor Name")]
        public string Name { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string? ExistingProfilePicture { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}