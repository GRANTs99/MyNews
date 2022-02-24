using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
