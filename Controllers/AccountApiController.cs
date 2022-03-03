using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using MyNews.Repository;
using MyNews.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Avatar> _contextAvatar;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountApiController(UserManager<User> userManager, SignInManager<User> signInManager, IRepository<Avatar> contextAv, RoleManager<IdentityRole> roleManager)
        {
            _contextAvatar = contextAv;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _userManager.Users.ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<User>> Get(string name)
        {
            User user = await _userManager.FindByNameAsync(name);
            if (user == null)
                return NotFound();
            return new ObjectResult(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            User user = new User { Email = model.Email, UserName = model.UserName };
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
            var result = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "simpleuser");
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(user);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();

                }
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<User>> Put(ChangePasswordViewModel model)
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
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest(model);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<User>> Put(EditUserViewModel model)
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
                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{name}")]
        public async Task<ActionResult<User>> Delete(string Name)
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
                    return NoContent();
                }
                return BadRequest();
            }
            return NotFound();
        }
    }
}
