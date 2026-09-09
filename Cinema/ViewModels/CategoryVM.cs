using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters long.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
    }
}