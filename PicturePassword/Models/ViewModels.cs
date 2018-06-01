using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PicturePassword.data;

namespace PicturePassword.Models
{
    public class ImageLinkViewModels
    {
        public string ImageLink { get; set; }
        public string Password { get; set; }
    }
    public class SingleImageVM
    {
        public bool canVisit { get; set; }
        public UploadedPicture picture { get; set; }
        public string message { get; set; }
    }
    public class MyAccountVM
    {
        public IEnumerable<UploadedPicture> pictures { get; set; }
        public User user { get; set; }
        public MyAccountVM()
        {
            var pictures = new List<UploadedPicture>();
        }
    }
}