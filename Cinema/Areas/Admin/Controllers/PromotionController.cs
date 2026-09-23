using AbsoluteCinema.Models;
using AbsoluteCinema.Services;
using AbsoluteCinema.Utility;
using AbsoluteCinema.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area(RoleConstants.ADMIN)]
    [Authorize(Roles = RoleConstants.ADMIN + "," + RoleConstants.SUPER_ADMIN)]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IMovieService _movieService;
        private readonly IUnitOfWork _unitOfWork;
        public PromotionController(IPromotionService promotionService, IMovieService movieService , IUnitOfWork unitOfWork)
        {
            _promotionService = promotionService;
            _movieService = movieService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var promotions = _promotionService.GetAllPromotions();
            return View(promotions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var promotionVM = new PromotionVM
            {
                Movies = _movieService.GetAll().Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                })
            };
            return View(promotionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PromotionVM vm)
        {
            if (ModelState.IsValid)
            {
                var promotion = vm.Adapt<Promotion>();

                _promotionService.CreatePromotion(promotion);
                _unitOfWork.promotionRepository.CommitAsync();


                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _movieService.GetAll().Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            });
            return View(vm);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var promotion = _promotionService.GetPromotionById(id);
            if (promotion == null) return NotFound();
            
            var vm = new PromotionVM
            {
                Id = promotion.Id,
                Code = promotion.Code,
                Discount = promotion.Discount,
                MaxUsage = promotion.MaxUsage,
                TimesUsed = promotion.TimesUsed,
                ValidTo = promotion.ValidTo,
                MovieId = promotion.MovieId,
                Status = promotion.Status,
                Movies = _movieService.GetAll().Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                })
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(PromotionVM vm)
        {
            Console.WriteLine("UPDATE ACTION HIT");

            if (ModelState.IsValid)
            {
                var promotion = vm.Adapt<Promotion>();
                _promotionService.UpdatePromotion(promotion);
                //_unitOfWork.promotionRepository.Commit();

                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _movieService.GetAll().Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            });
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _promotionService.DeletePromotion(id);
            return RedirectToAction(nameof(Index));
        }
    }
}