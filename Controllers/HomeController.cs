using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNews.Models;
using MyNews.Repository;
using MyNews.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Publication> _contextPublication;
        private readonly IRepository<PublicationItem> _contextPublicationItem;
        private readonly UserManager<User> _userManager;

        public HomeController(IRepository<Publication> contextP, IRepository<PublicationItem> contextPI, UserManager<User> userManager)
        {
            _contextPublication = contextP;
            _contextPublicationItem = contextPI;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_contextPublication.GetAll());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UserAccount(string name)
        {
            if (name == User.Identity.Name)
            {
                return RedirectToAction("My", "Account");
            }
            User user = _userManager.FindByNameAsync(name).Result;
            if (user == null)
            {
                return NotFound();
            }
            UserAccountViewModel model = new UserAccountViewModel { User = user };
            return View(model);
        }
    }
}
