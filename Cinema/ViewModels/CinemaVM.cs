using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AbsoluteCinema.ViewModels
{
    public class CinemaVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required.")]
        [Display(Name = "Cinema Name")]
        public string Name { get; set; } = string.Empty;


        public string Address { get; set; } = string.Empty;

        public string? ExistingCinemaLogo { get; set; }

        [Display(Name = "Cinema Logo")]
        public IFormFile? CinemaLogo { get; set; }

        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
    }
}