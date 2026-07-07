using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}