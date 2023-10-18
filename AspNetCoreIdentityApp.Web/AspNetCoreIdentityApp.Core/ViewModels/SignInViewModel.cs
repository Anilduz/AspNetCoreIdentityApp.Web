using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {

        }
        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
