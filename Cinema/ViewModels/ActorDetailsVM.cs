using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels
{
    public class ActorDetailsVM
    {
        public int ActorId { get; set; }
        [Required(ErrorMessage = "Actor name is required.")]
        [Length(2, 100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string ActorName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public int MoviesCount { get; set; }
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
    }
}