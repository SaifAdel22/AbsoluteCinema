using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } 
        public List<Movie> Movies { get; set; } = new List<Movie>();

    }
}
