using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbsoluteCinema.Models
{
	public class Cinema
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Cinema name is required.")]
		[StringLength(150, ErrorMessage = "Cinema name cannot exceed 150 characters.")]
		public string Name { get; set; }

		public string? Img { get; set; }

		public ICollection<Movie> Movies { get; set; } = new List<Movie>();
	}
}