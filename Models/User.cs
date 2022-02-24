using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Models
{
    public class User : IdentityUser
    {
        //Сделать UserName уникальным
        public List<Publication> Publications { get; set; }
        public Avatar Avatar { get; set; }
        [ForeignKey("AvatarId")]
        public int AvatarId { get; set; }
    }
}
