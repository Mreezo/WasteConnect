using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using WasteConnect.Models;
using WasteConnect.Services;
using WasteConnect.ViewModel;
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

            if (await _userManager.IsInRoleAsync(user, "Councillor"))
            {
                return RedirectToAction("Dashboard", "Councillor");
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

        [HttpGet]
        public async Task<IActionResult> SetCouncillorPassword(
    string userId,
    string token)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!await _userManager.IsInRoleAsync(user, "Councillor"))
            {
                return RedirectToAction(nameof(Login));
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                TempData["PasswordResetSuccess"] =
                    "Your password has already been created.";

                return RedirectToAction(nameof(Login));
            }

            var decodedToken =
                Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token));

            var model = new CounsellorPasswordViewModel
            {
                UserId = userId,
                Token = decodedToken
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult InvalidCouncillorInvitation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCouncillorPassword( CounsellorPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var councillor = await _userManager.FindByIdAsync(model.UserId);

            if (councillor == null)
            {
                return RedirectToAction(nameof(InvalidCouncillorInvitation));
            }

            var isCouncillor = await _userManager.IsInRoleAsync(
                councillor,
                "Councillor");

            if (!isCouncillor)
            {
                return RedirectToAction(nameof(InvalidCouncillorInvitation));
            }

            // Stop the invitation link from being used again.
            if (!string.IsNullOrWhiteSpace(councillor.PasswordHash))
            {
                TempData["LoginMessage"] =
                    "Your password has already been created. Please log in.";

                return RedirectToAction(nameof(Login));
            }

            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                return RedirectToAction(nameof(InvalidCouncillorInvitation));
            }

            var passwordResult =
                await _userManager.ResetPasswordAsync(
                    councillor,
                    decodedToken,
                    model.Password);

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // Activate the councillor account.

            councillor.EmailConfirmed = true;
            councillor.IsAccountActive = true;

            var updateResult = await _userManager.UpdateAsync(councillor);

            var loginUrl = Url.Action(
                "Login",
                "Account",
                null,
                Request.Scheme);

            await _emailService.SendCouncillorAccountActivatedAsync(
                councillor.Email!,
                councillor.FullName!,
                loginUrl!);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            return RedirectToAction(
                nameof(CouncillorPasswordCreated),
                new { userId = councillor.Id });
        }

        [HttpGet]
        public async Task<IActionResult> CouncillorPasswordCreated(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var councillor = await _userManager.FindByIdAsync(userId);

            if (councillor == null)
            {
                return RedirectToAction(nameof(Login));
            }

            ViewBag.FullName = councillor.FullName;

            return View();
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }   }
}