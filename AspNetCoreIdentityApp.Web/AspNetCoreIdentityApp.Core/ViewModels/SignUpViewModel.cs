using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel() { 
        }
        public SignUpViewModel(string userName, string email, string phone, string password)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
        }

        [Required(ErrorMessage = "Kullanıcı adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Telefon Boş Bırakılamaz")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre")]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor! Lütfen tekrar giriniz!")]
        [Required(ErrorMessage = "Şifre tekrar Boş Bırakılamaz")]
        [Display(Name = "Şifre Tekrar")]
        public string? PasswordConfirm { get; set; }


    }
}
