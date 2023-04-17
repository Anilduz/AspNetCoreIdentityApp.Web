using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localization;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }).AddUserValidator<UserValidator>().AddPasswordValidator<PasswordValidator>().AddErrorDescriber<LocalizationIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
