using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WasteConnect.Models;
using WasteConnect.Services;
using WasteConnect.ViewModels;

namespace WasteConnect.Controllers
{
    [Authorize(Roles = "Councillor")]
    public class CouncillorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ReportCosmosService _reportCosmosService;

        public CouncillorController(
            UserManager<ApplicationUser> userManager,
            ReportCosmosService reportCosmosService)
        {
            _userManager = userManager;
            _reportCosmosService = reportCosmosService;
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

            if (!councillor.WardNumber.HasValue)
            {
                TempData["Error"] =
                    "Your councillor account has not been linked to a ward.";

                var emptyViewModel = new CouncillorDashboardViewModel
                {
                    CouncillorName = councillor.FullName,
                    WardNumber = 0
                };

                return View(emptyViewModel);
            }

            var reports =
                await _reportCosmosService.GetReportsByWardAsync(
                    councillor.WardNumber.Value);

            var viewModel = new CouncillorDashboardViewModel
            {
                CouncillorName = councillor.FullName,
                Email = councillor.Email ?? string.Empty,
                PositionTitle = councillor.PositionTitle ?? "Ward Councillor",
                WardNumber = councillor.WardNumber.Value,
                Reports = reports
            };

            return View(new CouncillorDashboardViewModel
            {
                CouncillorName = councillor.FullName,
                Email = councillor.Email ?? string.Empty,
                PositionTitle = councillor.PositionTitle ?? "Ward Councillor",
                WardNumber = 0
            });


        }
    }
}