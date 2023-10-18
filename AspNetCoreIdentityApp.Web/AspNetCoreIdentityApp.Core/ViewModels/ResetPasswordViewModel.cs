using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Yeni Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre")]
        public string ?Password{ get; set; }

        [Compare(nameof(Password), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor! Lütfen tekrar giriniz!")]
        [Required(ErrorMessage = "Yeni Şifre tekrar Boş Bırakılamaz")]
        [Display(Name = "Şifre Tekrar")]
        public string? PasswordConfirm { get; set; }

    }
}
