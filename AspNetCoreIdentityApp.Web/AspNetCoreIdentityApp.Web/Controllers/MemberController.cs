using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using UserViewModel = AspNetCoreIdentityApp.Core.ViewModels.UserViewModel;
using AspNetCoreIdentityApp.Core.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));

        }
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel req)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (! await _memberService.CheckPasswordAsync(userName,req.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış.");
                return View();
            }
            var (isSuccess,errors) = await _memberService.ChangePasswordAsync(userName, req.OldPassword,req.PasswordNew);
            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir.";

            return View();


        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;
            message = "Bu işlemi yapmak için yetkiniz bulunmamaktadır. Yetki almak için yöneticiniz ile görüşebilirsiniz";
            ViewBag.message = message;
            return View();
        }
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.getGenderSelectList();
            
            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel req)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess,errors) = await _memberService.EditUserAsync(req,userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }
            

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla güncellenmiştir.";




            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }
       
        public async Task Logout()
        {
            _memberService.LogoutAsync();
            //return RedirectToAction("Index","Home");
        }
    }
}
