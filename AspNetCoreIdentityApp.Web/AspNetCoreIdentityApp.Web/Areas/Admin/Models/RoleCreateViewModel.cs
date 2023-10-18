using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Role ismi boş bırakılamaz!")]
        [Display(Name = "Role ismim : ")]
        public string RoleName { get; set; }
    }
}
