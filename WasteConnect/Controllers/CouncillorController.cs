using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WasteConnect.Models;

namespace WasteConnect.Controllers
{
    [Authorize(Roles = "Councillor")]
    public class CouncillorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CouncillorController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var councillor = await _userManager.GetUserAsync(User);

            if (councillor == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!councillor.IsAccountActive)
            {
                await HttpContext.SignOutAsync(
                    IdentityConstants.ApplicationScheme);

                TempData["LoginError"] =
                    "Your councillor account is not active.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View(councillor);
        }
    }
}