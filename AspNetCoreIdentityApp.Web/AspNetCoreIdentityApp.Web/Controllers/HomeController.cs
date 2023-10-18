using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text.RegularExpressions;
using AspNetCoreIdentityApp.Core.Models;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _SignInManager;

        private readonly IEmailService _emailService;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _UserManager = userManager;
            _SignInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {

            try
            {
                returnUrl = returnUrl ?? Url.Action("Index", "Member");
                var hasUser = await _UserManager.FindByEmailAsync(model.Email);
                if (hasUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                    return View();
                }

                var signInResult = await _SignInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

                if (signInResult.Succeeded)
                {
                    return Redirect(returnUrl!);
                }

                if (signInResult.IsLockedOut)
                {
                    ModelState.AddModelErrorList(new List<string>() { "Çok fazla giriş denemesi yaptınız, 3 dakika boyunca giriş yapamazsınız." });
                    return View();
                }

                ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış", $"Başarısız giriş sayısı = {await _UserManager.GetAccessFailedCountAsync(hasUser)}" });
                return View();


            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Beklenmedik bir hata oluştu: " + ex.Message.ToString();
                return View();
            }

        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var identityResult = await _UserManager.CreateAsync(new()
                {
                    UserName = request.UserName,
                    PhoneNumber = request.Phone,
                    Email = request.Email
                }, request.PasswordConfirm!);



                if (identityResult.Succeeded)
                {

                    TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleşmiştir. ";
                    return RedirectToAction(nameof(HomeController.SignUp));
                }

                //foreach (IdentityError item in identityResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item.Description);
                //}

                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Beklenmedik bir hata oluştu: " + ex.Message.ToString();
                return View();
            }
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel req)
        {
            var hasUser = await _UserManager.FindByEmailAsync(req.Email!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı bulunamadı!");
                return View();
            }

            string passwordResetToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            //for (int i = 0; i < 100; i++)    
            //{
            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);
            //}

            TempData["success"] = "Şifre yenileme linki, eposta adresinize gönderilmiştir.";
            return RedirectToAction(nameof(ForgetPassword));




            //email service ihtiyacı


            // örnek https://localhost:7244?userId=12312&token=aaasmdqwkmdqwdmq
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request) {
                   //public async IActionResult ResetPassword(ResetPasswordViewModel request)
        //{
            var userId = TempData["userId"];
            var token = TempData["token"];

            if(userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi!");
            }
            var hasUser = await _UserManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            { 
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır!");
                return View();
            }
            IdentityResult result = await _UserManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password!);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir!";
                
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                
            }
            return View();

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> getDocument()
        {
            var path = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot\\images\\4.pdf");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", "Demo.pdf");
        }

        [HttpPost]
        public string GetGlobalIpAdress()
        {
            try
            {
                var webClient = new WebClient();

                string dnsString = webClient.DownloadString("http://checkip.dyndns.org");
                dnsString = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(dnsString).Value;

                webClient.Dispose();
                return dnsString;
            }
            catch
            {
                throw;
            }
        }
    }
}