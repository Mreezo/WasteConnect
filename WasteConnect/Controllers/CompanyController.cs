using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WasteConnect.Models;
using WasteConnect.Services;
using WasteConnect.ViewModels;

namespace WasteConnect.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BlobService _blobService;
        private readonly CompanyCosmosService _companyService;
        private readonly ReportCosmosService _reportService;
        private readonly IConfiguration _configuration;

        public CompanyController(
            UserManager<ApplicationUser> userManager,
            BlobService blobService,
            CompanyCosmosService companyService,
            ReportCosmosService reportService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _blobService = blobService;
            _companyService = companyService;
            _reportService = reportService;
            _configuration = configuration;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterCompany()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var existingCompany = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (existingCompany != null)
            {
                return RedirectToAction(nameof(MyCompany));
            }

            var model = new CompanyRegistrationViewModel
            {
                ContactPerson = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber 
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCompany(CompanyRegistrationViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var existingCompany = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (existingCompany != null)
            {
                TempData["Error"] = "You have already registered a company.";
                return RedirectToAction(nameof(MyCompany));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string companyRegistrationUrl =
                    await _blobService.UploadPdfAsync(model.CompanyRegistration);

                string wasteLicenseUrl =
                    await _blobService.UploadPdfAsync(model.WasteLicense);

                string taxClearanceUrl =
                    await _blobService.UploadPdfAsync(model.TaxClearance);

                string? insuranceCertificateUrl = null;

                if (model.InsuranceCertificate != null)
                {
                    insuranceCertificateUrl =
                        await _blobService.UploadPdfAsync(model.InsuranceCertificate);
                }

                var company = new Company
                {
                    UserId = user.Id,
                    CompanyName = model.CompanyName,
                    RegistrationNumber = model.RegistrationNumber,
                    ContactPerson = model.ContactPerson,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    ServiceArea = model.ServiceArea,
                    Description = model.Description,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    CompanyRegistrationUrl = companyRegistrationUrl,
                    WasteLicenseUrl = wasteLicenseUrl,
                    TaxClearanceUrl = taxClearanceUrl,
                    InsuranceCertificateUrl = insuranceCertificateUrl
                };

                await _companyService.AddCompanyAsync(company);

                TempData["CompanySuccess"] =
                    "Company registration submitted successfully. Your application is pending admin approval.";

                return RedirectToAction(nameof(RegisterCompany));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyCompany()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (company == null)
            {
                TempData["Info"] = "Please register your company first.";
                return RedirectToAction(nameof(RegisterCompany));
            }

            return View(company);
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> AssignedJobs()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.AzureMapsKey = _configuration["AzureMaps:SubscriptionKey"];

            var jobs = await _reportService
                .GetReportsByAssignedCompanyAsync(user.Id);

            return View(jobs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJobStatus(string reportId, string reportUserId, string status)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService.GetReportByIdAsync(
                reportId,
                reportUserId);

            if (report == null)
                return NotFound();

            if (report.AssignedCompanyUserId != user.Id)
                return Forbid();

            if (status != "In Progress" && status != "Cleaned")
            {
                TempData["JobError"] = "Invalid status selected.";
                return RedirectToAction(nameof(AssignedJobs));
            }

            report.Status = status;

            await _reportService.UpdateMasterAndLinkedReportsAsync(report); ;

            TempData["JobSuccess"] =
                $"Job status updated to {status}.";

            return RedirectToAction(nameof(AssignedJobs));
        }
        [HttpGet]
        public async Task<IActionResult> CompleteJob(string id, string userId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService.GetReportByIdAsync(id, userId);

            if (report == null)
                return NotFound();

            if (report.AssignedCompanyUserId != user.Id)
                return Forbid();

            if (report.Status != "In Progress")
            {
                TempData["JobError"] =
                    "Only jobs that are In Progress can be completed.";

                return RedirectToAction(nameof(AssignedJobs));
            }

            var model = new CompleteJobViewModel
            {
                ReportId = report.Id,
                ReportUserId = report.UserId,
                BeforeImageUrl = report.ImageUrl,
                DumpingLocation = report.DumpingLocation,
                Status = report.Status
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteJob(CompleteJobViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var report = await _reportService.GetReportByIdAsync(
                model.ReportId,
                model.ReportUserId);

            if (report == null)
                return NotFound();

            if (report.AssignedCompanyUserId != user.Id)
                return Forbid();

            string? afterImageUrl = null;

            if (model.AfterCleanupImage != null)
            {
                afterImageUrl =
                    await _blobService.UploadFileAsync(model.AfterCleanupImage);
            }
            else if (!string.IsNullOrEmpty(model.CapturedAfterImageData))
            {
                afterImageUrl =
                    await _blobService.UploadBase64ImageAsync(model.CapturedAfterImageData);
            }

            if (string.IsNullOrEmpty(afterImageUrl))
            {
                ModelState.AddModelError("", "Please upload or capture an after-cleanup photo.");

                model.BeforeImageUrl = report.ImageUrl;
                model.DumpingLocation = report.DumpingLocation;
                model.Status = report.Status;

                return View(model);
            }

            report.AfterCleanupImageUrl = afterImageUrl;
            report.CompletionNotes = model.CompletionNotes;
            report.CompletedAt = DateTime.UtcNow;
            report.Status = "Cleaned";

            await _reportService.UpdateMasterAndLinkedReportsAsync(report); ;

            TempData["JobSuccess"] =
                "Cleanup completed successfully. Proof photo submitted.";

            return RedirectToAction(nameof(CompletedJobs));
        }
        [HttpGet]
        public async Task<IActionResult> CompletedJobs()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var jobs = await _reportService
                .GetReportsByAssignedCompanyAsync(user.Id);

            var completedJobs = jobs
                .Where(j => j.Status == "Cleaned")
                .OrderByDescending(j => j.CompletedAt)
                .ToList();

            return View(completedJobs);
        }

        [HttpGet]
        public async Task<IActionResult> CompanyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (company == null)
            {
                return RedirectToAction(nameof(RegisterCompany));
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (company == null)
                return RedirectToAction(nameof(RegisterCompany));

            if (profileImage == null || profileImage.Length == 0)
            {
                TempData["ProfileError"] = "Please select an image to upload.";
                return RedirectToAction(nameof(CompanyProfile));
            }

            var imageUrl = await _blobService.UploadFileAsync(profileImage);

            company.ProfileImageUrl = imageUrl;

            await _companyService.UpdateCompanyAsync(company);

            TempData["ProfileSuccess"] = "Company profile picture updated successfully.";

            return RedirectToAction(nameof(CompanyProfile));
        }
        [HttpGet]
        public async Task<IActionResult> EditCompanyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);

            if (company == null)
                return RedirectToAction(nameof(RegisterCompany));

            var model = new CompanyProfileViewModel
            {
                Id = company.Id,
                UserId = company.UserId,
                CompanyName = company.CompanyName,
                RegistrationNumber = company.RegistrationNumber,
                ContactPerson = company.ContactPerson,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                ServiceArea = company.ServiceArea,
                ProfileImageUrl = company.ProfileImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompanyProfile(CompanyProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var company = await _companyService.GetCompanyByIdAsync(model.Id, model.UserId);

            if (company == null)
                return NotFound();

            company.CompanyName = model.CompanyName;
            company.RegistrationNumber = model.RegistrationNumber;
            company.ContactPerson = model.ContactPerson;
            company.Email = model.Email;
            company.PhoneNumber = model.PhoneNumber;
            company.ServiceArea = model.ServiceArea;

            await _companyService.UpdateCompanyAsync(company);

            TempData["ProfileSuccess"] = "Company profile updated successfully.";

            return RedirectToAction(nameof(CompanyProfile));
        }

        
    }
}