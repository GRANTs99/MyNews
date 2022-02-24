using Microsoft.AspNetCore.Http;
using MyNews.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.ViewModels.Publication
{
    public class EditPublicationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<PublicationItem> Items { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public IFormFileCollection Img { get; set; }
        [Display(Name = "Text")]
        public List<string> Text { get; set; }
        [Required]
        public List<string> items { get; set; }
    }
}
