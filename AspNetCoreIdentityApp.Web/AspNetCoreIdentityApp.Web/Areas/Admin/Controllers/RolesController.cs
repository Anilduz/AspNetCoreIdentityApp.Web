using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;


        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task <IActionResult> Index()
        {
            var roles =  await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();
            return View(roles);
        }
        public IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "admin,role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel req)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = req.RoleName });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            return RedirectToAction(nameof(RolesController.Index));
        }
        [Authorize(Roles = "admin,role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if(roleToUpdate == null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır.");
            }
            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate.Name});
        }

        [Authorize(Roles ="admin,role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel req)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(req.Id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır.");
            }
            roleToUpdate.Name = req.Name;
            await _roleManager.UpdateAsync(roleToUpdate);
            ViewData["SuccessMessage"] = "Role bilgisi güncellenmiştir";
            return View();
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate == null)
            {
                throw new Exception("Silinecek role bulunamamıştır.");
            }
            return View(new RoleDeleteViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate.Name });
        }
        [Authorize(Roles = "admin,role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleDelete(RoleDeleteViewModel req)
        {
            var roletodelete = await _roleManager.FindByIdAsync(req.Id);
            if (roletodelete == null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır.");
            }
            roletodelete.Name = req.Name;
            await _roleManager.DeleteAsync(roletodelete);
            ViewData["SuccessMessage"] = "Role başarıyla silindi!";
            return RedirectToAction(nameof(RolesController.Index));

        }
        
        [Authorize(Roles ="admin,role-action")]

        public async Task<IActionResult> AssignRoleToUser(string id)
        {   
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();


            var roleViewModelList = new List<AssignRoleToUserViewModel>();


            var userRoles = await _userManager.GetRolesAsync(currentUser); 
            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }
                roleViewModelList.Add(assignRoleToUserViewModel);
            }
                

            return View(roleViewModelList);
        }
        [Authorize(Roles = "admin,role-action")]
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = await _userManager.FindByIdAsync(userId);
            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            return RedirectToAction("UserList", "Home");
        }
    }
}
