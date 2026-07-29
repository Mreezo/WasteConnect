using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WasteConnect.Models;
using WasteConnect.Services;

namespace WasteConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWardLookupService _wardLookupService;

        public HomeController(ILogger<HomeController> logger, IWardLookupService wardLookupService)
        {
            _logger = logger;
            _wardLookupService = wardLookupService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> TestWard(
            double latitude,
            double longitude)
        {
            var wardNumber =
                await _wardLookupService.FindWardNumberAsync(
                    latitude,
                    longitude);

            if (wardNumber == null)
            {
                return Content(
                    $"No Msunduzi ward was found for " +
                    $"Latitude: {latitude}, Longitude: {longitude}");
            }

            return Content(
                $"The location belongs to Msunduzi Ward {wardNumber}.");
        }
    }
}
