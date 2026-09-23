using AbsoluteCinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{

    [Area(AreaConstants.CUSTOMER_AREA)]
    [Authorize]

    public class FavoriteController : Controller
    {
        public IFavoriteService _favoriteService;
        public IMovieService _movieService;
        public UserManager<ApplicationUser> _userManager;
        public FavoriteController(IFavoriteService favoriteService, IMovieService movieService, UserManager<ApplicationUser> userManager)
        {
            _favoriteService = favoriteService;
            _movieService = movieService;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var favorites = _favoriteService.GetAllFavorites(_userManager.GetUserId(User));
            return View("Index", favorites);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            try
            {
                await _favoriteService.AddToFavorites(userId, movieId);
                TempData["Success"] = "Movie added to favorites successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int movieId) 
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            try
            {
                var favorite = _favoriteService.GetFavorite(userId, movieId);

                if (favorite != null)
                {
                    await _favoriteService.RemoveFromFavorites(favorite);
                    TempData["Success"] = "Movie removed from favorites successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
