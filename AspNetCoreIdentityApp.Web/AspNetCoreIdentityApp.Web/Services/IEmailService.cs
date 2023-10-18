using Microsoft.Extensions.Options;

namespace AspNetCoreIdentityApp.Web.Services
{
    public interface IEmailService
    {
        
        Task SendResetPasswordEmail(string resetEmailLink, string ToEmail);
    }
}
