using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class CompleteJobViewModel
    {
        public string ReportId { get; set; }

        public string ReportUserId { get; set; }

        public string? BeforeImageUrl { get; set; }

        public string DumpingLocation { get; set; }

        public string Status { get; set; }

        [Display(Name = "Completion Notes")]
        public string? CompletionNotes { get; set; }

        [Display(Name = "Upload After Cleanup Photo")]
        public IFormFile? AfterCleanupImage { get; set; }

        public string? CapturedAfterImageData { get; set; }
    }
}