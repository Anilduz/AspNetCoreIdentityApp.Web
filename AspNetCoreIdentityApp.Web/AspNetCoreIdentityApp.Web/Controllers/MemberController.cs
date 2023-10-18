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

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            var userViewModel = new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture,
            };

            return View(userViewModel);
        }
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel req)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, req.OldPassword);
            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış.");
                return View();
            }
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, req.OldPassword, req.PasswordNew);
            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, req.PasswordNew, true, false);
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
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = (await _userManager.FindByNameAsync(User.Identity.Name));
            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirthDay = currentUser.BirthDay,
                Gender = currentUser.Gender


            };
            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel req)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            currentUser.UserName = req.UserName;
            currentUser.Email = req.Email;
            currentUser.PhoneNumber = req.Phone;
            currentUser.City = req.City;
            currentUser.BirthDay = req.BirthDay;
            currentUser.Gender = req.Gender;

            if (req.Picture != null && req.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(req.Picture.FileName)}";
                if (req.Picture.FileName.Contains("jpeg") || req.Picture.FileName.Contains("jpg") || req.Picture.FileName.Contains("png"))
                {
                    var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "UserPictures").PhysicalPath!, randomFileName);

                    using var stream = new FileStream(newPicturePath, FileMode.Create);
                    await req.Picture.CopyToAsync(stream);


                    currentUser.Picture = randomFileName;
                }
                else
                {
                    TempData["ErrorImage"] = "Görsel yüklenemedi.";
                }
                var updateToUserResult = await _userManager.UpdateAsync(currentUser);
                if (!updateToUserResult.Succeeded)
                {
                    ModelState.AddModelErrorList(updateToUserResult.Errors);
                    return View();

                }
            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);
            TempData["SuccessMessage"] = "Üye bilgileri başarıyla güncellenmiştir.";


            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirthDay = currentUser.BirthDay,
                Gender = currentUser.Gender
            };

            return View(userEditViewModel);
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            //return RedirectToAction("Index","Home");
        }
    }
}
