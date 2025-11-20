using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tsg.UI.Main.Models
{
    // Note: doesn't expose events or behavior
    public class ShortenedUrlResponse
    {
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }

        //public static ShortenedUrlResponse FromShortenedUrl(ShortenedUrl item)
        //{
        //    return new ShortenedUrlResponse()
        //    {
        //        LongUrl = item.LongUrl,
        //        ShortUrl = item.Domain +"/"+ item.Key
        //    };
        //}
    }
}
