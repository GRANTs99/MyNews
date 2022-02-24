using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNews.Models;
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
        private readonly ApplicationContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PublicationApiController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> Get()
        {
            return await _context.Publications.Include(p => p.Items).ToListAsync();
        }

        [HttpGet("{id}")]
        public ActionResult<Publication> Get(int id)
        {
            Publication post = _context.Publications.Where(p => p.Id == id).FirstOrDefault();
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
                            _context.PublicationItems.Add(item);
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
                        _context.PublicationItems.Add(item);
                        textcount++;
                    }
                }
                _context.Publications.Add(post);
                _context.SaveChanges();

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
                Publication post = _context.Publications.Where(p => p.Id == model.Id).FirstOrDefault();
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
                                _context.PublicationItems.Add(item);
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
                            _context.PublicationItems.Add(item);
                            textcount++;
                        }
                    }
                    _context.Publications.Update(post);
                    _context.SaveChanges();
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
            Publication post = _context.Publications.Where(p => p.Id == id).FirstOrDefault();
            if (post != null)
            {
                if (User.Identity.Name != post.User.UserName)
                {
                    return Unauthorized();
                }
                foreach(var p in post.Items)
                {
                    _context.PublicationItems.Remove(p);
                }
                _context.Publications.Remove(post);
                return NoContent();
            }
            return NotFound();
        }
    }
}

