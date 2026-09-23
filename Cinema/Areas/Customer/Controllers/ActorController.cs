using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{
    [Area(AreaConstants.CUSTOMER_AREA)]

    public class ActorController : Controller
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public IActionResult Index(string query, int pageNumber = 1)
        {
            int pageSize = 5;
            var actors = _actorService.GetPagedActors(query, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.Query = query;

            return View(actors);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var actor = _actorService.GetActorDetails(id);
            if (actor == null) return NotFound();

            return View(actor);
        }

       
        
    }
}