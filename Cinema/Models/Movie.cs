using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
	public class Movie
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Movie title is required.")]
		[StringLength(200, ErrorMessage = "Movie title cannot exceed 200 characters.")]
		public string Name { get; set; }

		public string Description { get; set; }

		[Range(0.01, 10000.00, ErrorMessage = "Price must be greater than 0.")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Price { get; set; }

		public bool Status { get; set; }

		[Required(ErrorMessage = "Show date and time is required.")]
		public DateTime DateTime { get; set; }

		public string MainImg { get; set; }

		[Required(ErrorMessage = "Category is required.")]
		public int CategoryId { get; set; }
		public Category Category { get; set; }

		[Required(ErrorMessage = "Cinema is required.")]
		public int CinemaId { get; set; }
		public Cinema Cinema { get; set; }

		public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
		public List<MovieSubImg> SubImgs { get; set; } = new List<MovieSubImg>();
	}
}