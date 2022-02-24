using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyNews.Models;
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
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public PublicationController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                            _context.PublicationItems.Add(item);
                            imgcount++;
                        }
                        else
                        {
                            //Не удалось загрузить картинку
                            return View(pvm);
                        }

                    }
                    if (p == "Text")
                    {
                        var item = new PublicationItem(post, "Text", pvm.Text.Skip(textcount).FirstOrDefault(), null);
                        _context.PublicationItems.Add(item);
                        textcount++;
                    }
                }
                _context.Publications.Add(post);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(pvm);
        }

        public IActionResult Edit(int id)
        {
            Publication post = _context.Publications.Where(p => p.Id == id).FirstOrDefault();
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
                Publication post = _context.Publications.Where(p => p.Id == model.Id).FirstOrDefault();
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
                                _context.PublicationItems.Add(item);
                                imgcount++;
                            }
                            else
                            {
                                //Не удалось загрузить картинку
                                return View(model);
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
            Publication post = _context.Publications.Where(p => p.Id == id).FirstOrDefault();
            if (post != null)
            {
                if (User.Identity.Name != post.User.UserName)
                {
                    return Unauthorized();
                }
                foreach (var p in post.Items)
                {
                    _context.PublicationItems.Remove(p);
                }
                _context.Publications.Remove(post);
                return RedirectToAction("My","Account");
            }
            return NotFound();
        }
    }
}
