using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using MyNews.Repository;
using MyNews.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Controllers
{
    [Authorize(Roles = "moderator")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Publication> _contextPublication;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IRepository<Publication> contextP)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _contextPublication = contextP;
        }
        [Authorize(Roles = "admin")]
        public IActionResult UserList() => View(_userManager.Users.ToList());
        public async Task<IActionResult> EditRole(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return RedirectToAction("UserList");
            }

            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("UserList");
        }

        public IActionResult PublicationList() => View(_contextPublication.GetAll());//Include(PublicationItems)
        [HttpPost]
        public ActionResult DeletePublication(int id)
        {
            Publication post = _contextPublication.Get(id);
            if (post != null)
            {
                _contextPublication.Remove(post);
                _contextPublication.Save();
            }
            return RedirectToAction("PublicationList");
        }
    }
}
