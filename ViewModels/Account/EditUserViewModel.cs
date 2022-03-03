using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.ViewModels.Account
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IFormFile Avatar { get; set; }

        public byte[] ConvertAvatar()
        {
            if (Avatar != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)Avatar.Length);
                }
                return imageData;
            }
            else
            {
                return null;
            }
        }
    }
}
