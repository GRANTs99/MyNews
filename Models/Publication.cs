using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Models
{
    public class Publication
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public List<PublicationItem> Items { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Like> Likes { get; set; }
        public int LikeCount { get {return Likes.Count; } }

    }
}
