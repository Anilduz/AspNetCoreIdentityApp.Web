using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class UserEditViewModel
    {

        [Required(ErrorMessage = "Kullanıcı adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [Display(Name = "Email")]
        public string? Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon Boş Bırakılamaz")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; } = null!;

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }


        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [Display(Name = "Resim")]
        public IFormFile? Picture { get; set; }
    
        
        [Display(Name = "Cinsiyet")]
        public Gender? Gender { get; set; } 





    }
}
