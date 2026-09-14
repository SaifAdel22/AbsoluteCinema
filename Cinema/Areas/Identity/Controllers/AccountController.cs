using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.Utility;
using AbsoluteCinema.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Identity.Controllers
{
[Area(AreaConstants.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        // Service layer => UserStore<ApplicationUser>
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;



        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,IUnitOfWork unitOfWork,
            IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);


            ApplicationUser user = registerVM.Adapt<ApplicationUser>(/*config*/);

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(registerVM);
            }

            {
                
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(Confirm), ControllerConstants.ACCOUNT_CONTROLLER, new { area = AreaConstants.IDENTITY_AREA, user.Id, token }, Request.Scheme);
                string body = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";

                await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account", body);
            }

            TempData["SuccessMessage"] = "Account created successfully! Please login.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Confirm(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                TempData[NotificationConstants.ERROR_NOTIFICATION] = String.Join(", ", result.Errors.Select(e => e.Description));
            else
            {
                TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirm Account successfully, please login";
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ??
                                    await _userManager.FindByNameAsync(loginVM.EmailOrUserName);

            if (user is null)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }

            

            var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.Remember, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Too many attempts, please try again later");
            }
            if (signInResult.IsNotAllowed)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Please verify your account!");
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }

         

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Welcome Back {user.FirstName} {user.LastName}";

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        /***
         * TODO
         */
        [HttpGet]
        public IActionResult ResendConfirmation()
        {
            // generate view

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmation(ResendEmailConfirmationVM model)
        {
            // 1. generate new token
            // 2. generate link
            // 3. generate new body
            if (!ModelState.IsValid)
                return View(model);

            if (model.EmailOrUserName is null)
            {
                ModelState.AddModelError(nameof(model.EmailOrUserName), "Email Or UserName is required");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.EmailOrUserName) ??
                                    await _userManager.FindByNameAsync(model.EmailOrUserName);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "If your email is registered, a confirmation link has been sent.");
                return View(model);
            }
            if(await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Your email is already confirmed.");
                return RedirectToAction(nameof(Login));
            }



            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), ControllerConstants.ACCOUNT_CONTROLLER, new { area = AreaConstants.IDENTITY_AREA, user.Id, token }, Request.Scheme);
            string body = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";
            // 4. send email


            // 5. redirect to login

            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account", body);

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirmation email resent. Please check your inbox.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Logout successfully";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                                    await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);

            if (user is null)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");

                return View(forgetPasswordVM);
            }

            string otp = new Random().Next(1000, 9999).ToString();

            await _applicationUserOTPRepository.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                OTP = otp,
            }, ct);
            await _applicationUserOTPRepository.CommitAsync(ct);


            string body = $"<h1>Your otp number is: {otp}. don't share it.</h1>";
            await _emailSender.SendEmailAsync(user.Email!, "Reset your account", body);

            TempData["RedirectToValidateOTP"] = Guid.NewGuid();
            Response.Cookies.Append("userId", user.Id);

            return RedirectToAction(nameof(ValidateOTP));
        }

        [HttpGet]
        public IActionResult ValidateOTP()
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "" });
            if (TempData["RedirectToValidateOTP"] is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTP)
        {
            if (!ModelState.IsValid)
                return View(validateOTP);

            var userId = Request.Cookies["userId"];
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var otpInDB = _applicationUserOTPRepository
                .Get(e => e.ApplicationUserId == userId && !e.IsUsed && e.ValidTo >= DateTime.UtcNow)
                .OrderBy(e => e.CreateAt)
                .LastOrDefault();

            if (otpInDB is null || otpInDB.OTP is null || validateOTP.OTP != otpInDB.OTP)
            {
                TempData[NotificationConstants.ERROR_NOTIFICATION] = $"Invalid OTP";

                TempData["RedirectToValidateOTP"] = Guid.NewGuid();
                return RedirectToAction(nameof(ValidateOTP));
            }

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Valid OTP, you can now change your password";
            otpInDB.IsUsed = true;
            await _applicationUserOTPRepository.CommitAsync();

            return RedirectToAction(nameof(NewPassword));
        }

        [HttpGet]
        public IActionResult NewPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(newPasswordVM);

            var userId = Request.Cookies["userId"];
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(newPasswordVM);
            }

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Reset Password successfully";
            Response.Cookies.Delete("userId");

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ExternalLogin()
        {
            return View();
        }

       

    }
}
