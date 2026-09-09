using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        
        public IRepository<Cinema> _repository;
        public IRepository<Movie> _movierepository;
        public CinemaController(IRepository<Cinema> repository , IRepository<Movie> movierepository)
        {
            _repository = repository;
            _movierepository = movierepository;
        }
        [HttpGet]
        public IActionResult Index(string? query, int pageNumber = 1)
        {
            int pageSize = 5;
            var cinemas = _repository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                cinemas = cinemas.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            int totalItems = cinemas.Count();
            var pagedCinemas = cinemas.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var cinemaVMs = pagedCinemas.Select(c => new CinemaVM
            {
                Id = c.Id,
                Name = c.Name,
                ExistingCinemaLogo = c.Img,
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchQuery = query;

            return View(cinemaVMs);
        }

        public IActionResult Details(int id)
        {

            var cinema = _repository.GetOne(c => c.Id == id);
            var cinemamovies = _movierepository.Get(expression: m => m.CinemaId == id).Select
                
                (m=> new MovieVM
                {
                    Id = m.Id,
                    Title = m.Name,
                    ExistingMainImg = m.MainImg,
                    Price = m.Price,
                    Description = m.Description

                }).ToList();

            var cinemaVM = new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                ExistingCinemaLogo = cinema.Img,
                Movies = cinemamovies

            };


            return View(cinemaVM);
        }
    }
}
