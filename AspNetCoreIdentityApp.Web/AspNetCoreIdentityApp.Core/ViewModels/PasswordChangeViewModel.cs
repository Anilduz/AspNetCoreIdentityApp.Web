using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre")]
        [MinLength(6, ErrorMessage ="Şifreniz 6 karakterden uzun olmalıdır.")]
        public string OldPassword { get; set; } = null!;

        [MinLength(6, ErrorMessage = "Şifreniz 6 karakterden uzun olmalıdır.")]
        [Required(ErrorMessage = "Yeni Şifre Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre")]
        public string PasswordNew { get; set; } = null!;

        [MinLength(6, ErrorMessage = "Şifreniz 6 karakterden uzun olmalıdır.")]
        [Compare(nameof(PasswordNew), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor! Lütfen tekrar giriniz!")]
        [Required(ErrorMessage = "Yeni Şifre tekrar Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
