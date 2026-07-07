using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WasteConnect.Services;
using Microsoft.AspNetCore.Mvc;
using WasteConnect.Models;
using WasteConnect.ViewModels;

namespace WasteConnect.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IMemoryCache _cache;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<  ApplicationUser> signInManager, EmailService emailService,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _cache = cache;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser? user =
                await _userManager.FindByEmailAsync(model.EmailOrPhone);

            if (user == null)
            {
                user = _userManager.Users
                    .FirstOrDefault(u => u.PhoneNumber == model.EmailOrPhone);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email/phone number or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email/phone number or password.");
                return View(model);
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                return RedirectToAction("Dashboard", "Company");
            }

            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                return RedirectToAction("Dashboard", "User");
            }

            await _signInManager.SignOutAsync();

            ModelState.AddModelError("", "Your account does not have a valid role.");
            return View(model);
        }


        // REGISTER

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Role != "User" && model.Role != "Company")
            {
                ModelState.AddModelError("", "Please select a valid registration type.");
                return View(model);
            }

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View(model);
            }

            var existingPhone = _userManager.Users
                .FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);

            if (existingPhone != null)
            {
                ModelState.AddModelError("", "An account with this phone number already exists.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                TempData["RegisterSuccess"] = "Registration successful. You can now login.";
                return RedirectToAction(nameof(Register));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // FORGOT PASSWORD


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please enter a valid email address."
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No account exists with this email address."
                });
            }

            var code = new Random().Next(100000, 999999).ToString();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            _cache.Set($"ResetCode_{model.Email}", code, TimeSpan.FromMinutes(10));
            _cache.Set($"ResetToken_{model.Email}", token, TimeSpan.FromMinutes(10));

            await _emailService.SendPasswordResetCodeAsync(model.Email, code);

            return Json(new
            {
                success = true,
                message = "Verification code sent successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyResetCodeAjax(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email and code are required."
                });
            }

            var savedCode = _cache.Get<string>($"ResetCode_{email}");

            if (string.IsNullOrEmpty(savedCode) || savedCode != code)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid or expired verification code."
                });
            }

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("ResetPassword", "Account", new { email })
            });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var token = _cache.Get<string>($"ResetToken_{email}");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("ForgotPassword");

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid password reset request.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.NewPassword);

            if (result.Succeeded)
            {
                _cache.Remove($"ResetCode_{model.Email}");
                _cache.Remove($"ResetToken_{model.Email}");

                TempData["PasswordResetSuccess"] =
                 "Password changed successfully. You can now login.";

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }   }
}