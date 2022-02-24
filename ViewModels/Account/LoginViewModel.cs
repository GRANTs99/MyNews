using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Rememder me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
