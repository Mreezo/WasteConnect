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
        private readonly IConfiguration _configuration;

        public CouncillorController(
            UserManager<ApplicationUser> userManager,
            ReportCosmosService reportCosmosService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _reportCosmosService = reportCosmosService;
            _configuration = configuration;
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
                    Email = councillor.Email ?? string.Empty,
                    PositionTitle =
                        councillor.PositionTitle ?? "Ward Councillor",
                    WardNumber = 0,
                    Reports = new List<DumpingReport>()
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

            return View(viewModel);



        }

        [HttpGet]
        public async Task<IActionResult> ReportDetails(
            string id,
            string userId)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(userId))
            {
                return NotFound();
            }

            var councillor =
                await _userManager.GetUserAsync(User);

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
                    "Your councillor account is not linked to a ward.";

                return RedirectToAction(nameof(Dashboard));
            }

            var report =
                await _reportCosmosService.GetReportByIdAsync(
                    id,
                    userId);

            if (report == null)
            {
                return NotFound();
            }

            // Security check:
            // Councillors can only view reports in their own ward.
            if (report.WardNumber != councillor.WardNumber.Value)
            {
                return Forbid();
            }

            // Only show master reports on the councillor portal.
            if (!report.IsMasterReport)
            {
                return NotFound();
            }

            ViewBag.AzureMapsKey =
                _configuration["AzureMaps:SubscriptionKey"];

            return View(report);
        }
    }
}