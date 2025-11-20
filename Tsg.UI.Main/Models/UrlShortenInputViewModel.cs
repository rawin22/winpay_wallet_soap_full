using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tsg.UI.Main.Models
{
    public class UrlShortenInputViewModel
    {
        [Display(Name = "URL")]
        public string Url { get; set; }
        [Display(Name = "Shortened URL")]
        public string ShortenedUrl { get; set; }
    }
}
