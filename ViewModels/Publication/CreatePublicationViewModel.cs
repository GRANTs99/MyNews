using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.ViewModels.Publication
{
    public class CreatePublicationViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public IFormFileCollection Img { get; set; }
        [Display(Name = "Text")]
        public List<string> Text { get; set; }
        [Required]
        public List<string> items { get; set; }
    }
}