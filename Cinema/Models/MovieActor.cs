using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.Models
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; }

        [Required(ErrorMessage = "Character name is required.")]
        [StringLength(150, ErrorMessage = "Character name cannot exceed 150 characters.")]
        public string CharacterName { get; set; }
    }
}
