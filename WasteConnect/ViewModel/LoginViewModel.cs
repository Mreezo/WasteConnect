using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email or phone number is required.")]
        [Display(Name = "Email or Phone Number")]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}