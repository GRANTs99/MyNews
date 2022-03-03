using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNews.Models;
using MyNews.Repository;
using MyNews.ViewModels.Publication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Publication> _contextPublication;
        private readonly IRepository<PublicationItem> _contextPublicationItem;
        private readonly IRepository<Like> _contextLike;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PublicationApiController(UserManager<User> userManager, SignInManager<User> signInManager, IRepository<Publication> contextP, IRepository<PublicationItem> contextPI, IRepository<Like> contextL, RoleManager<IdentityRole> roleManager)
        {
            _contextPublication = contextP;
            _contextPublicationItem = contextPI;
            _contextLike = contextL;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> Get()
        {
            var posts = await _contextPublication.GetAllAsync();
            return new ObjectResult(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Publication>> Get(int id)
        {
            Publication post = await _contextPublication.GetAsync(id);
            if (post == null)
                return NotFound();
            return new ObjectResult(post);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Publication> Post(CreatePublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Publication post = new Publication { Title = model.Title, Date = DateTime.Now, User = _userManager.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefault() };
                int imgcount = 0;
                int textcount = 0;
                byte[] imageData = null;
                foreach (var p in model.items)
                {
                    if (p == "Img")
                    {
                        if (model.Img != null)
                        {
                            using (var binaryReader = new BinaryReader(model.Img.Skip(imgcount).FirstOrDefault().OpenReadStream()))
                            {
                                imageData = binaryReader.ReadBytes((int)model.Img.Skip(imgcount).FirstOrDefault().Length);
                            }
                            var item = new PublicationItem(post, "Img", model.Img.Skip(imgcount).FirstOrDefault().FileName, imageData);
                            _contextPublicationItem.Add(item);
                            imgcount++;
                        }
                        else
                        {
                            return BadRequest();
                        }

                    }
                    if (p == "Text")
                    {
                        var item = new PublicationItem(post, "Text", model.Text.Skip(textcount).FirstOrDefault(), null);
                        _contextPublicationItem.Add(item);
                        textcount++;
                    }
                }
                _contextPublication.Add(post);
                _contextPublication.Save();

                return Ok(post);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPut]
        public ActionResult<Publication> Put(EditPublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Publication post = _contextPublication.GetAll().Where(p => p.Id == model.Id).FirstOrDefault();
                if (post != null)
                {
                    if (post.User.UserName != User.Identity.Name)
                    {
                        return NotFound();
                    }
                    post.Items.Clear();
                    int imgcount = 0;
                    int textcount = 0;
                    byte[] imageData = null;
                    foreach (var p in model.items)
                    {
                        if (p == "Img")
                        {
                            if (model.Img != null)
                            {
                                using (var binaryReader = new BinaryReader(model.Img.Skip(imgcount).FirstOrDefault().OpenReadStream()))
                                {
                                    imageData = binaryReader.ReadBytes((int)model.Img.Skip(imgcount).FirstOrDefault().Length);
                                }
                                var item = new PublicationItem(post, "Img", model.Img.Skip(imgcount).FirstOrDefault().FileName, imageData);
                                _contextPublicationItem.Add(item);
                                imgcount++;
                            }
                            else
                            {
                                return BadRequest();
                            }

                        }
                        if (p == "Text")
                        {
                            var item = new PublicationItem(post, "Text", model.Text.Skip(textcount).FirstOrDefault(), null);
                            _contextPublicationItem.Add(item);
                            textcount++;
                        }
                    }
                    _contextPublication.Update(post);
                    _contextPublication.Save();
                    return Ok(post);
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<Publication> Delete(int id)
        {
            Publication post = _contextPublication.GetAll().Where(p => p.Id == id).FirstOrDefault();
            if (post != null)
            {
                if (User.Identity.Name != post.User.UserName)
                {
                    return Unauthorized();
                }
                foreach(var p in post.Items)
                {
                    _contextPublicationItem.Remove(p);
                }
                _contextPublication.Remove(post);
                return NoContent();
            }
            return NotFound();
        }
    }
}

