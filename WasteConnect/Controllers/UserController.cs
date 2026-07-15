using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WasteConnect.Models;
using WasteConnect.Services;
using WasteConnect.ViewModels;

namespace WasteConnect.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BlobService _blobService;
        private readonly ReportCosmosService _reportService;
        private readonly IConfiguration _configuration;
        private readonly TwilioOtpService _twilioOtpService;


        public UserController(
            UserManager<ApplicationUser> userManager,
            BlobService blobService,
            ReportCosmosService reportService,
            IConfiguration configuration,
             TwilioOtpService twilioOtpService)
        {
            _userManager = userManager;
            _blobService = blobService;
            _reportService = reportService;
            _configuration = configuration;
            _twilioOtpService = twilioOtpService;
        }

        private double CalculateDistanceMeters( double lat1,double lon1,double lat2,double lon2)
        {
            const double earthRadius = 6371000;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) *
                Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            double c =
                2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private string CalculatePriority(int duplicateCount)
        {
            if (duplicateCount >= 50)
                return "High";

            if (duplicateCount >= 20)
                return "Medium";

            return "Low";
        }

        private string NormalizeLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return "";

            return location
                .ToLower()
                .Replace(",", " ")
                .Replace(".", " ")
                .Replace("-", " ")
                .Trim();
        }

        private bool IsSimilarLocation(string newLocation, string existingLocation)
        {
            var newWords = NormalizeLocation(newLocation)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .ToList();

            var existingWords = NormalizeLocation(existingLocation)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .ToList();

            int matchingWords = newWords.Intersect(existingWords).Count();

            return matchingWords >= 2 ||
                   NormalizeLocation(newLocation).Contains(NormalizeLocation(existingLocation)) ||
                   NormalizeLocation(existingLocation).Contains(NormalizeLocation(newLocation));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new UserProfileViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                HomeAddress = user.HomeAddress
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.ProfileImageUrl = user.ProfileImageUrl;
                model.Email = user.Email;
                model.HomeAddress = user.HomeAddress;

                
                return View(model);
            }

            if (model.ProfileImage != null)
            {
                user.ProfileImageUrl =
                    await _blobService.UploadFileAsync(model.ProfileImage);
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.HomeAddress = model.HomeAddress;


            await _userManager.UpdateAsync(user);

            TempData["ProfileSuccess"] =
                "Profile updated successfully.";

            return RedirectToAction(nameof(Profile));
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DisposalSite()
        {
            return View();
        }

        // ======================
        // Report Dumping
        // ======================

        [HttpGet]
        public async Task<IActionResult> ReportDumping()
        {
            var user = await _userManager.GetUserAsync(User);

             ViewBag.AzureMapsKey =
            _configuration["AzureMaps:SubscriptionKey"];

            var model = new DumpingReportViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                City = "Pietermaritzburg"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReport(DumpingReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AzureMapsKey =
                    _configuration["AzureMaps:SubscriptionKey"];

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x =>
                        $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}");

                TempData["Error"] = string.Join(" | ", errors);

                return View("ReportDumping", model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            string? imageUrl = null;

            if (model.ImageFile != null)
            {
                imageUrl = await _blobService.UploadFileAsync(model.ImageFile);
            }
            else if (!string.IsNullOrEmpty(model.CapturedImageData))
            {
                imageUrl = await _blobService.UploadBase64ImageAsync(model.CapturedImageData);
            }

            double dumpLat = double.Parse(
                model.DumpLatitude.Replace(",", "."),
                CultureInfo.InvariantCulture);

            double dumpLng = double.Parse(
                model.DumpLongitude.Replace(",", "."),
                CultureInfo.InvariantCulture);

            var openReports =
                await _reportService.GetOpenMasterReportsAsync();

            DumpingReport? duplicateReport =
            openReports.FirstOrDefault(r =>
                r.DumpLatitude != 0 &&
                r.DumpLongitude != 0 &&
                CalculateDistanceMeters(
                    dumpLat,
                    dumpLng,
                    r.DumpLatitude,
                    r.DumpLongitude) <= 1000 &&
                IsSimilarLocation(
                    model.DumpingLocation,
                    r.DumpingLocation));

            if (duplicateReport != null)
            {
                duplicateReport.LastReportedAt = DateTime.UtcNow;

                duplicateReport.ReportedUserIds ??= new List<string>();

                if (!duplicateReport.ReportedUserIds.Contains(user.Id))
                {
                    duplicateReport.ReportedUserIds.Add(user.Id);
                    duplicateReport.DuplicateCount += 1;
                }

                duplicateReport.Priority =
                    CalculatePriority(duplicateReport.DuplicateCount);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    duplicateReport.AdditionalImageUrls ??= new List<string>();
                    duplicateReport.AdditionalImageUrls.Add(imageUrl);
                }

                await _reportService.UpdateReportAsync(duplicateReport);

                var userLinkedReport = new DumpingReport
                {
                    UserId = user.Id,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    DumpingLocation = model.DumpingLocation,
                    City = "Pietermaritzburg",
                    ImageUrl = imageUrl,
                    Status = duplicateReport.Status,
                    CreatedAt = DateTime.UtcNow,

                    DumpLatitude = dumpLat,
                    DumpLongitude = dumpLng,

                    DuplicateCount = duplicateReport.DuplicateCount,
                    Priority = duplicateReport.Priority,
                    LastReportedAt = duplicateReport.LastReportedAt,

                    IsMasterReport = false,
                    MasterReportId = duplicateReport.Id,
                    MasterReportUserId = duplicateReport.UserId,

                    AssignedCompanyId = duplicateReport.AssignedCompanyId,
                    AssignedCompanyUserId = duplicateReport.AssignedCompanyUserId,
                    AssignedCompanyName = duplicateReport.AssignedCompanyName,
                    AssignedAt = duplicateReport.AssignedAt
                };

                await _reportService.AddReportAsync(userLinkedReport);

                TempData["ReportMessage"] =
                $"This area has already been reported. Your report was added to the existing case. Current status: {duplicateReport.Status}. Priority: {duplicateReport.Priority}. Reported by {duplicateReport.DuplicateCount} people.";

                return RedirectToAction(nameof(MyReports));     
            }

            var report = new DumpingReport
            {
                UserId = user.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                DumpingLocation = model.DumpingLocation,
                City = "Pietermaritzburg",
                ImageUrl = imageUrl,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,

                DumpLatitude = dumpLat,
                DumpLongitude = dumpLng,

                DuplicateCount = 1,
                Priority = "Low",
                LastReportedAt = DateTime.UtcNow,
                IsMasterReport = true,
                MasterReportId = null,
                MasterReportUserId = null,
                ReportedUserIds = new List<string> { user.Id },
                AdditionalImageUrls = new List<string>()
            };

            await _reportService.AddReportAsync(report);

            TempData["ReportMessage"] =
                "Illegal dumping report submitted successfully.";

            return RedirectToAction(nameof(MyReports));
        }

        // ======================
        // MY REPORTS
        // ======================

        [HttpGet]
        public async Task<IActionResult> MyReports()
        {
            var user =
                await _userManager.GetUserAsync(User);

            var reports =
                await _reportService
                    .GetReportsByUserIdAsync(user.Id);

            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> EditReport(string id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService
                .GetReportByIdAsync(id, user.Id);

            if (report == null)
                return NotFound();

            if (report.Status != "Pending")
            {
                TempData["EditError"] =
                    "Only pending reports can be edited.";

                return RedirectToAction(nameof(MyReports));
            }

            var model = new DumpingReportViewModel
            {
                FullName = report.FullName,
                PhoneNumber = report.PhoneNumber,
                DumpingLocation = report.DumpingLocation,
                City = report.City
            };

            ViewBag.ReportId = report.Id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReport( string id,DumpingReportViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService
                .GetReportByIdAsync(id, user.Id);

            if (report == null)
                return NotFound();

            if (report.Status != "Pending")
            {
                TempData["Error"] =
                    "Only pending reports can be edited.";

                return RedirectToAction(nameof(MyReports));
            }

            report.DumpingLocation =
                model.DumpingLocation;

            if (model.ImageFile != null)
            {
                report.ImageUrl =
                    await _blobService.UploadFileAsync(
                        model.ImageFile);
            }
            else if (!string.IsNullOrEmpty(
                model.CapturedImageData))
            {
                report.ImageUrl =
                    await _blobService.UploadBase64ImageAsync(
                        model.CapturedImageData);
            }

            await _reportService.UpdateReportAsync(report);

            TempData["Success"] =
                "Report updated successfully.";

            return RedirectToAction(nameof(MyReports));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteReport(string id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService.GetReportByIdAsync(id, user.Id);

            if (report == null)
                return NotFound();

            if (report.Status != "Pending" &&
                report.Status != "Cleaned")
            {
                TempData["ReportError"] =
                    "This report can no longer be deleted because it has already been assigned for cleanup.";

                return RedirectToAction(nameof(MyReports));
            }

            await _reportService.DeleteReportAsync(id, user.Id);

            TempData["DeleteSuccess"] = "Your report has been deleted successfully.";

            return RedirectToAction(nameof(MyReports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReportOtp(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Phone number is required."
                });
            }

            await _twilioOtpService.SendReportOtpAsync(phoneNumber);

            return Json(new
            {
                success = true,
                message = "OTP sent successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyReportOtp(string phoneNumber, string otp)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(otp))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Phone number and OTP are required."
                });
            }

            bool isValid =
                _twilioOtpService.VerifyReportOtp(phoneNumber, otp);

            if (!isValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid or expired OTP."
                });
            }

            return Json(new
            {
                success = true,
                message = "OTP verified successfully."
            });
        }
    }
}