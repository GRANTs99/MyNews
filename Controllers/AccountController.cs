using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyNews.Models;
using Microsoft.AspNetCore.Identity;
using MyNews.ViewModels.Account;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyNews.Repository;

namespace MyNews.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Avatar> _contextAvatar;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IRepository<Avatar> contextAv, RoleManager<IdentityRole> roleManager)
        {
            _contextAvatar = contextAv;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName};
                if (model.Avatar != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.Avatar.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.Avatar.Length);
                    }
                    Avatar avatar = new Avatar { User = user, Data = imageData, FileName = model.Avatar.FileName };
                    _contextAvatar.Add(avatar);
                    user.Avatar = avatar;
                }
                else
                {
                    byte[] imageData = System.IO.File.ReadAllBytes("default.png");
                    Avatar avatar = new Avatar { User = user, Data = imageData, FileName = "default.png" };
                    _contextAvatar.Add(avatar);
                    user.Avatar = avatar;

                }
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "simpleuser");
                if (result.Succeeded)
                {
                    _contextAvatar.Save();
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and/or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult My()
        {
            User user = _userManager.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefault();
            var model = new UserMyViewModel { User = user };
            return View(model);
        }

        public IActionResult ChangePassword(User user)
        {
            if (user == null)
            {
                return NotFound();
            }
            if (user.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("My");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return View(model);
        }

        public IActionResult Edit()
        {
            User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, Name = user.UserName};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Name;
                    if (model.Avatar != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(model.Avatar.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.Avatar.Length);
                        }
                        Avatar avatar = new Avatar { User = user, Data = imageData, FileName = model.Avatar.FileName };
                        _contextAvatar.Add(avatar);
                        _contextAvatar.Save();
                        user.Avatar = avatar;
                    }
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("My");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }
        public IActionResult Delete()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Name)
        {
            if (User.Identity.Name != Name)
            {
                return NotFound();
            }
            User user = _userManager.Users.Where(p => p.UserName == Name).FirstOrDefault();
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
                return BadRequest();
            }
            return NotFound();
        }

    }
}