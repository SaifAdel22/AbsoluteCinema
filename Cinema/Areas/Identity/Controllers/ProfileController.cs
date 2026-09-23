using AbsoluteCinema.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace AbsoluteCinema.Areas.Identity.Controllers
{
    [Area (AreaConstants.IDENTITY_AREA)]
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            var userVm = new UserProfileVM();
            user.Adapt(userVm);
            userVm.CurrentPassword = string.Empty;
            userVm.NewPassword = string.Empty;
            userVm.ConfirmNewPassword = string.Empty;

            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserProfileVM userProfileVM)
        {
            // 1. لو خانات الباسورد فاضية، بنشيل أخطائها من الـ ModelState عشان يعدي كأنه بيعدل اسم وعنوان بس
            if (string.IsNullOrEmpty(userProfileVM.NewPassword) &&
                string.IsNullOrEmpty(userProfileVM.CurrentPassword) &&
                string.IsNullOrEmpty(userProfileVM.ConfirmNewPassword))
            {
                ModelState.Remove(nameof(userProfileVM.CurrentPassword));
                ModelState.Remove(nameof(userProfileVM.NewPassword));
                ModelState.Remove(nameof(userProfileVM.ConfirmNewPassword));
            }

            if (!ModelState.IsValid)
            {
                return View(userProfileVM);
            }

            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // 2. لو اليوزر كتب باسورد جديد، هنا يبدأ لوجيك التغيير بحذر شديد
            if (!string.IsNullOrEmpty(userProfileVM.NewPassword))
            {
                // التأكد من تطابق الجديد مع التأكيد أولاً
                if (userProfileVM.NewPassword != userProfileVM.ConfirmNewPassword)
                {
                    ModelState.AddModelError("ConfirmNewPassword", "The new password and confirmation password do not match.");
                    return View(userProfileVM);
                }

                if (string.IsNullOrEmpty(userProfileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to set a new password.");
                    return View(userProfileVM);
                }

                var passwordCheck = await _signInManager.UserManager.CheckPasswordAsync(user, userProfileVM.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(userProfileVM);
                }

                var passwordChangeResult = await _signInManager.UserManager.ChangePasswordAsync(user, userProfileVM.CurrentPassword, userProfileVM.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var err in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                    return View(userProfileVM);
                }
            }

            user.FirstName = userProfileVM.FirstName;
            user.LastName = userProfileVM.LastName;
            user.Address = userProfileVM.Address;

            var result = await _signInManager.UserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(userProfileVM);
            }

            if (!string.IsNullOrEmpty(userProfileVM.NewPassword))
            {
                await _signInManager.UserManager.UpdateSecurityStampAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Updated Successfully";

            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });
        }
    }
}
