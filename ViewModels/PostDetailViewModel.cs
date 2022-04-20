using System;
using System.Collections.Generic;
using BlogYou.Models;

namespace BlogYou.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
