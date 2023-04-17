using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() {  Code = "DuplicateUserName", Description=$"{userName} kullanıcı adı daha önceden alınmıştır."};
            //return base.DuplicateUserName(userName);
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateEmail", Description = $"{email} mail adresi ile daha önceden kullanıcı oluşturulmuştur." };
            //return base.DuplicateEmail(email);
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordToShort", Description = $"Şifre en az 6 karakter olmalıdır!!" };
            //return base.PasswordTooShort(length);
        }
        public override IdentityError PasswordMismatch()
        {
            return new() { Code = "PasswordToMismacth", Description = $"Şifreleriniz uyuşmuyor! Lütfen şifrelerinizi tekrar giriniz." };
            //return base.PasswordMismatch();
        }

    }
}
