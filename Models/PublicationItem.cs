using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Models
{
    public class PublicationItem
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        public Publication Publication { get; set; }
        public int PublicationId { get; set; }
        public string Text { get; set; }
        public byte[] Data { get; set; }

        public PublicationItem()
        {

        }
        public PublicationItem(Publication publication, string type, string text, byte[] data)
        {
            Text = text;
            Publication = publication;
            Data = data;
            Type = type;
        }
    }
}
