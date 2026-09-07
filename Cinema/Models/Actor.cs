using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Actor name is required.")]
        [StringLength(150, ErrorMessage = "Actor name cannot exceed 150 characters.")]
        public string Name { get; set; }

        public string? Img { get; set; }
        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters.")]
        public string? Nationality { get; set; }
        public DateOnly? DOB { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
