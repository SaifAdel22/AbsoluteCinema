using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels
{
    public class ActorVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Actor name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public IFormFile? ProfileImgFile { get; set; }

        public string? ExistingProfileImg { get; set; }
    }
}