using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasteConnect.Services;

namespace WasteConnect.Controllers
{
    
    public class DisposalSiteController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DisposalSiteService _disposalSiteService;

        public DisposalSiteController(
            DisposalSiteService disposalSiteService, IConfiguration configuration)
        {
            _disposalSiteService = disposalSiteService;
            _configuration = configuration;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult FindDisposalSite()
        {
            ViewBag.AzureMapsKey =
         _configuration["AzureMaps:SubscriptionKey"];

            var sites =
                _disposalSiteService.GetDisposalSites();

            return View(sites);
        }


        [Authorize(Roles = "Company")]
        public IActionResult CompanyDisposalSite()
        {
            ViewBag.AzureMapsKey =
         _configuration["AzureMaps:SubscriptionKey"];

            var sites =
                _disposalSiteService.GetDisposalSites();

            return View(sites);
        }

        
    }
}