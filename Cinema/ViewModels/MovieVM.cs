using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbsoluteCinema.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }

        public bool Status { get; set; } = true; 

        [Required(ErrorMessage = "Movie title is required.")]
        [Display(Name = "Movie Title")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000.")]
        public decimal Price { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

        // Foreign Keys
        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a cinema.")]
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }

        // Selected Actors for Many-to-Many
        [Display(Name = "Actors")]
        public List<int> SelectedActorIds { get; set; } = new List<int>();

        // Main Image Files
        public string? ExistingMainImg { get; set; }

        [Display(Name = "Main Image")]
        public IFormFile? MainImg { get; set; }

        // Additional Sub Images Files
        [Display(Name = "Sub Images")]
        public List<IFormFile>? NewSubImages { get; set; }
        public int? bio { get; set; }

        // Existing Sub Images for Edit view
        public List<MovieSubImgVM>? ExistingSubImages { get; set; }
        public List<ActorVM>? MovieActors { get; set; }

        public bool IsActive { get; set; } = true;
        public string CategoryName { get; set; } = string.Empty;

        // Select lists for dropdowns
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Cinemas { get; set; }
        public IEnumerable<SelectListItem>? Actors { get; set; }
    }
}