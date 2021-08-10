using GeniiApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeniiApp.Models;

namespace GeniiApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Authorize]
    public class UsersProfileController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        public UsersProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            var users = _userManager.Users;

            return View(users);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return RedirectToAction("UserManagement", _userManager.Users);

            var userRole = await _userManager.GetRolesAsync(user);

            string userRoleStr = userRole.FirstOrDefault().ToString();

            var data = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SurName = user.SurName,
                Email = user.Email,
                Role = userRole.FirstOrDefault()
            };


            var indexRole = string.Empty;
            var index0 = UserRole.Admin;
            var index1 = UserRole.Manager;
            var index2 = UserRole.User;

            if(userRoleStr == index0.ToString())
            {
                indexRole = "0";
            }

            if (userRoleStr == index1.ToString())
            {
                indexRole = "1";
            }

            if (userRoleStr == index2.ToString())
            {
                indexRole = "2";
            }

            var useRoleAndIndex = new { Index = indexRole, Name = userRoleStr };

            ViewData["UserRoles"] = useRoleAndIndex;

            var userCurrentRole = useRoleAndIndex.Name;

            TempData["userRole"] = userCurrentRole;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, [Bind("Email, Role, FirstName, SurName")] UserViewModel appUser)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                user.Email = appUser.Email;
                user.FirstName = appUser.FirstName;
                user.SurName = appUser.SurName;

                UserRole selectedRole = (UserRole)Enum.Parse(typeof(UserRole), appUser.Role);

                var currentUserRole = TempData["userRole"];

                await _userManager.RemoveFromRoleAsync(user, currentUserRole.ToString());

                await _userManager.AddToRoleAsync(user, selectedRole.ToString());

                var result = await _userManager.UpdateAsync(user);

                //await _signInManager.SignInAsync(user, false, "");

                if (result.Succeeded)
                    return RedirectToAction("UserManagement", _userManager.Users);

                ModelState.AddModelError("", "User not updated, something went wrong.");

                return View(appUser);
            }

            return RedirectToAction("UserManagement", _userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserViewModel userId)
        {
            var user = await _userManager.FindByIdAsync(userId.Id);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("UserManagement");
                else
                    ModelState.AddModelError("", "Something went wrong while deleting this user.");
            }
            else
            {
                ModelState.AddModelError("", "This user can't be found");
            }
            return View("UserManagement", _userManager.Users);
        }

    }
}
