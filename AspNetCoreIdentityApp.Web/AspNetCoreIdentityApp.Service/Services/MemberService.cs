using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        async public Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        async Task<UserViewModel> IMemberService.GetUserViewModelByUserNameAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            return new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture,
            };
        }

        async Task<bool> IMemberService.CheckPasswordAsync(string userName, string password)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return await _userManager.CheckPasswordAsync(currentUser, password);
        }

        async Task<(bool, IEnumerable<IdentityError>?)> IMemberService.ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);
            return (true, null);
        }

        async Task<UserEditViewModel> IMemberService.GetUserEditViewModelAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));
            return new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirthDay = currentUser.BirthDay,
                Gender = currentUser.Gender


            };
        }

        SelectList IMemberService.getGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        async Task<(bool, IEnumerable<IdentityError>?)> IMemberService.EditUserAsync(UserEditViewModel req, string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
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
                var updateToUserResult = await _userManager.UpdateAsync(currentUser);
                if (!updateToUserResult.Succeeded)
                {
                    return (false, updateToUserResult.Errors);
                }

            }
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);
            return (true, null);
        }
    }
}
