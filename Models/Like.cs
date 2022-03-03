﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId {get; set;}
        public User User { get; set; }
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }
    }
}
