using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
    public class PublicationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Publication> _contextPublication;
        private readonly IRepository<PublicationItem> _contextPublicationItem;
        private readonly IRepository<Like> _contextLike;

        public PublicationController(IRepository<Publication> contextP, IRepository<PublicationItem> contextPI, IRepository<Like> contextL, UserManager<User> userManager)
        {
            _contextPublication = contextP;
            _contextPublicationItem = contextPI;
            _userManager = userManager;
            _contextLike = contextL;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(CreatePublicationViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                Publication post = new Publication { Title = pvm.Title, Date = DateTime.Now, User = _userManager.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefault() };
                int imgcount = 0;
                int textcount = 0;
                byte[] imageData = null;
                foreach (var p in pvm.items)
                {
                    if (p == "Img")
                    {
                        if (pvm.Img != null)
                        {
                            using (var binaryReader = new BinaryReader(pvm.Img.Skip(imgcount).FirstOrDefault().OpenReadStream()))
                            {
                                imageData = binaryReader.ReadBytes((int)pvm.Img.Skip(imgcount).FirstOrDefault().Length);
                            }
                            var item = new PublicationItem(post, "Img", pvm.Img.Skip(imgcount).FirstOrDefault().FileName, imageData);
                            _contextPublicationItem.Add(item);
                            imgcount++;
                        }
                        else
                        {
                            return View(pvm);
                        }

                    }
                    if (p == "Text")
                    {
                        var item = new PublicationItem(post, "Text", pvm.Text.Skip(textcount).FirstOrDefault(), null);
                        _contextPublicationItem.Add(item);
                        textcount++;
                    }
                }
                _contextPublication.Add(post);
                _contextPublication.Save();

                return RedirectToAction("Index");
            }
            return View(pvm);
        }
        public IActionResult Edit(int id)
        {
            Publication post = _contextPublication.Get(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.User.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            EditPublicationViewModel model = new EditPublicationViewModel { Id = post.Id, Title =post.Title, Items = post.Items };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditPublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Publication post = _contextPublication.Get(model.Id);
                if (post != null)
                {
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
                                return View(model);
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
        public ActionResult<Publication> Delete(int id)
        {
            Publication post = _contextPublication.Get(id);
            if (post != null)
            {
                if (User.Identity.Name != post.User.UserName)
                {
                    return Unauthorized();
                }
                foreach (var p in post.Items)
                {
                    _contextPublicationItem.Remove(p);
                }
                _contextPublication.Remove(post);
                return RedirectToAction("My","Account");
            }
            return NotFound();
        }
        [HttpPost]
        public async void AddLike(int postId)
        {
            Publication post = _contextPublication.Get(postId);
            if (post != null)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                Like like = new Like { User = user, Publication = post };
                if (null == post.Likes.Where(p => p.User.UserName == user.UserName).FirstOrDefault())
                {
                    post.Likes.Add(like);
                    _contextLike.Add(like);
                }
                else
                {
                    post.Likes.Remove(like);
                    _contextLike.Remove(like);
                }
                _contextPublication.Update(post);
                _contextPublication.Save();
            }
        }
    }
}
