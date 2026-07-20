using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WasteConnect.Models;
using WasteConnect.Services;
using WasteConnect.ViewModel;
using WasteConnect.ViewModels;
namespace WasteConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CompanyCosmosService _companyService;
        private readonly ReportCosmosService _reportService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;


        public AdminController(
            ReportCosmosService reportService,
            CompanyCosmosService companyService,
             UserManager<ApplicationUser> userManager,
             IConfiguration configuration,
             EmailService emailService)
        {
            _reportService = reportService;
            _companyService = companyService;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reports(string? search, string? status)
        {
            var reports = await _reportService.GetAllReportsAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                reports = reports
                    .Where(r =>
                        (!string.IsNullOrEmpty(r.FullName) &&
                         r.FullName.ToLower().Contains(search)) ||

                        (!string.IsNullOrEmpty(r.DumpingLocation) &&
                         r.DumpingLocation.ToLower().Contains(search)) ||

                        (!string.IsNullOrEmpty(r.City) &&
                         r.City.ToLower().Contains(search)) ||

                        (!string.IsNullOrEmpty(r.PhoneNumber) &&
                         r.PhoneNumber.ToLower().Contains(search))
                    )
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                reports = reports
                    .Where(r => r.Status == status)
                    .ToList();
            }

            reports = reports
                .OrderByDescending(r => r.LastReportedAt)
                .ToList();

            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> ReportDetails(string id, string userId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
                return NotFound();

            var report = await _reportService.GetReportByIdAsync(id, userId);

            if (report == null)
                return NotFound();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCompanies(string? search)
        {
            var companies = await _companyService.GetAllCompaniesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                companies = companies
                    .Where(c =>
                        (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(c.ServiceArea) && c.ServiceArea.ToLower().Contains(search))
                    )
                    .ToList();
            }

            return View(companies);
        }

        [HttpGet]
        public async Task<IActionResult> CompanyDetails(string id, string userId)
        {
            var company = await _companyService.GetCompanyByIdAsync(id, userId);

            if (company == null)
                return NotFound();

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCompany(string id, string userId)
        {
            var company = await _companyService.GetCompanyByIdAsync(id, userId);

            if (company == null)
                return NotFound();

            company.Status = "Approved";
            company.RejectionReason = null;
            company.ReviewedAt = DateTime.UtcNow;

            await _companyService.UpdateCompanyAsync(company);

            TempData["CompanyActionSuccess"] =
                "Company application approved successfully.";

            return RedirectToAction(nameof(CompanyDetails), new
            {
                id = company.Id,
                userId = company.UserId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCompany(
            string id,
            string userId,
            string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["CompanyActionError"] =
                    "Please provide a reason for rejection.";

                return RedirectToAction(nameof(CompanyDetails), new
                {
                    id,
                    userId
                });
            }

            var company = await _companyService.GetCompanyByIdAsync(id, userId);

            if (company == null)
                return NotFound();

            company.Status = "Rejected";
            company.RejectionReason = rejectionReason;
            company.ReviewedAt = DateTime.UtcNow;

            await _companyService.UpdateCompanyAsync(company);

            TempData["CompanyActionSuccess"] =
                "Company application rejected successfully.";

            return RedirectToAction(nameof(CompanyDetails), new
            {
                id = company.Id,
                userId = company.UserId
            });
        }

        // ============================
        // ASSIGN JOB PAGE
        // ============================

        [HttpGet]
        public async Task<IActionResult> AssignJob(string reportId,string reportUserId)
        {
            if (string.IsNullOrEmpty(reportId) ||
                string.IsNullOrEmpty(reportUserId))
            {
                return NotFound();
            }

            var report =
                await _reportService.GetReportByIdAsync(
                    reportId,
                    reportUserId);

            if (report == null)
                return NotFound();

            var approvedCompanies =
                await _companyService.GetApprovedCompaniesAsync();

            var busyCompanyUserIds = new List<string>();

            foreach (var company in approvedCompanies)
            {
                bool isBusy =
                    await _reportService.CompanyHasActiveJobAsync(company.UserId);

                if (isBusy)
                {
                    busyCompanyUserIds.Add(company.UserId);
                }
            }

            ViewBag.Report = report;
            ViewBag.BusyCompanyUserIds = busyCompanyUserIds;

            return View(approvedCompanies);
        }

        // ============================
        // ASSIGN COMPANY TO REPORT
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCompanyToReport( string reportId,string reportUserId,string companyId,string companyUserId, DateTime cleanupStartDate,
            DateTime cleanupDueDate,
            string? cleanupInstructions)
        {
            var report =
                await _reportService.GetReportByIdAsync(
                    reportId,
                    reportUserId);

            if (report == null)
                return NotFound();

            if (report.Status == "Cleaned")
            {
                TempData["AssignError"] =
                    "This report has already been cleaned and cannot be assigned again.";

                return RedirectToAction(nameof(ReportDetails), new
                {
                    id = report.Id,
                    userId = report.UserId
                });
            }

            if (!string.IsNullOrEmpty(report.AssignedCompanyUserId) &&
                  report.Status != "Cleaned")
            {
                TempData["AssignError"] =
                    $"This report is already assigned to {report.AssignedCompanyName}. It cannot be assigned to another company.";

                return RedirectToAction(nameof(ReportDetails), new
                {
                    id = report.Id,
                    userId = report.UserId
                });
            }

            var company =
                await _companyService.GetCompanyByIdAsync(
                    companyId,
                    companyUserId);

            if (company == null)
                return NotFound();

            if (company.Status != "Approved")
            {
                TempData["AssignError"] =
                    "Only approved companies can be assigned cleanup jobs.";

                return RedirectToAction(nameof(AssignJob), new
                {
                    reportId,
                    reportUserId
                });
            }

            bool companyHasActiveJob =
                await _reportService.CompanyHasActiveJobAsync(company.UserId);

            if (companyHasActiveJob)
            {
                TempData["AssignError"] =
                    "This company already has an active cleanup job. They must complete their current job before receiving another one.";

                return RedirectToAction(nameof(AssignJob), new
                {
                    reportId,
                    reportUserId
                });
            }

            report.AssignedCompanyId = company.Id;
            report.AssignedCompanyUserId = company.UserId;
            report.AssignedCompanyName = company.CompanyName;
            report.AssignedAt = DateTime.UtcNow;
            report.Status = "Assigned";
            report.CleanupDueDate = cleanupDueDate;
            report.CleanupStartDate = cleanupStartDate;
            report.CleanupInstructions = cleanupInstructions;


            // Update master report AND all linked duplicate reports
            await _reportService.UpdateReportAsync(report);

            await _reportService.UpdateMasterAndLinkedReportsAsync(report);

            TempData["AssignSuccess"] =
                $"Cleanup job assigned to {company.CompanyName} successfully.";

            return RedirectToAction(nameof(ReportDetails), new
            {
                id = report.Id,
                userId = report.UserId
            });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers(string? search)
        {
            var allUsers = _userManager.Users.ToList();

            var communityUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    communityUsers.Add(user);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                communityUsers = communityUsers
                    .Where(u =>
                        (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(u.HomeAddress) && u.HomeAddress.ToLower().Contains(search))
                    )
                    .ToList();
            }

            return View(communityUsers);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCouncillors(string? search)
        {
            var allUsers = _userManager.Users.ToList();

            var councillors = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Councillor"))
                {
                    councillors.Add(user);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                councillors = councillors
                    .Where(c =>
                        (!string.IsNullOrEmpty(c.FullName) &&
                         c.FullName.ToLower().Contains(search)) ||

                        (!string.IsNullOrEmpty(c.Email) &&
                         c.Email.ToLower().Contains(search)) ||

                        (!string.IsNullOrEmpty(c.PhoneNumber) &&
                         c.PhoneNumber.ToLower().Contains(search)) ||

                        (c.WardNumber.HasValue &&
                         c.WardNumber.Value.ToString().Contains(search))
                    )
                    .ToList();
            }

            councillors = councillors
                .OrderBy(c => c.WardNumber)
                .ThenBy(c => c.FullName)
                .ToList();

            return View(councillors);
        }

        [HttpGet]
        public IActionResult CreateCounsellor()
        {
            return View(new CounsellorViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCouncillor(
           CounsellorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim().ToLower();

            var existingUser =
                await _userManager.FindByEmailAsync(normalizedEmail);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email address already exists.");

                return View(model);
            }

            var existingPhoneUser = _userManager.Users
                .FirstOrDefault(u =>
                    u.PhoneNumber == model.PhoneNumber.Trim());

            if (existingPhoneUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "An account with this phone number already exists.");

                return View(model);
            }

            var councillor = new ApplicationUser
            {
                FullName = model.FullName.Trim(),
                UserName = normalizedEmail,
                Email = normalizedEmail,
                PhoneNumber = model.PhoneNumber.Trim(),
                WardNumber = model.WardNumber,
                PositionTitle = model.PositionTitle.Trim(),
                IsAccountActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            // Create the account without a password.
            var createResult =
                await _userManager.CreateAsync(councillor);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var roleResult =
                await _userManager.AddToRoleAsync(
                    councillor,
                    "Councillor");

            if (!roleResult.Succeeded)
            {
                // Remove the incomplete account if role assignment fails.
                await _userManager.DeleteAsync(councillor);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            try
            {
                var passwordToken =
                    await _userManager.GeneratePasswordResetTokenAsync(
                        councillor);

                var encodedToken =
                    WebEncoders.Base64UrlEncode(
                        Encoding.UTF8.GetBytes(passwordToken));

                var setupLink = Url.Action(
                    action: "SetCouncillorPassword",
                    controller: "Account",
                    values: new
                    {
                        userId = councillor.Id,
                        token = encodedToken
                    },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(setupLink))
                {
                    throw new InvalidOperationException(
                        "The password setup link could not be generated.");
                }

                await _emailService.SendCouncillorPasswordSetupAsync(
                    councillor.Email!,
                    councillor.FullName,
                    councillor.WardNumber!.Value,
                    setupLink);

                TempData["CouncillorSuccess"] =
                    $"The councillor account was created successfully. " +
                    $"A password setup email was sent to {councillor.Email}.";

                return RedirectToAction(nameof(ManageCouncillors));
            }
            catch (Exception)
            {
                // Keep the account so the administrator can resend the email later.
                TempData["CouncillorWarning"] =
                    "The councillor account was created, but the password setup " +
                    "email could not be sent. You will be able to resend it.";

                return RedirectToAction(nameof(ManageCouncillors));
            }
        }



        [HttpGet]
        public async Task<IActionResult> UserProfile(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return View((user, roles));
        }

        [HttpGet]
        public IActionResult CommunityAnalytics()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReportAnalytics()
        {
            ViewBag.AzureMapsKey =
                _configuration["AzureMaps:SubscriptionKey"];

            var reports = await _reportService.GetAllReportsAsync();

            var model = new ReportAnalyticsViewModel
            {
                TotalReports = reports.Count,
                PendingReports = reports.Count(r => r.Status == "Pending"),
                AssignedReports = reports.Count(r => r.Status == "Assigned"),
                InProgressReports = reports.Count(r => r.Status == "In Progress"),
                CleanedReports = reports.Count(r => r.Status == "Cleaned"),

                MapPoints = reports
                    .Where(r => r.DumpLatitude != 0 && r.DumpLongitude != 0)
                    .Select(r => new ReportMapPoint
                    {
                        Location = r.DumpingLocation,
                        Status = r.Status,
                        Priority = r.Priority,
                        Latitude = r.DumpLatitude,
                        Longitude = r.DumpLongitude,
                        ReportedBy = r.DuplicateCount
                    })
                    .ToList(),

                WeeklyReports = reports
                    .GroupBy(r => r.CreatedAt.Date.AddDays(-(int)r.CreatedAt.DayOfWeek))
                    .OrderBy(g => g.Key)
                    .Select(g => new ReportTrendPoint
                    {
                        Label = g.Key.ToString("dd MMM"),
                        Count = g.Count()
                    })
                    .ToList(),

                MonthlyReports = reports
                    .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .Select(g => new ReportTrendPoint
                    {
                        Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Count = g.Count()
                    })
                    .ToList()
            };

            return View(model);
        }
    }
}