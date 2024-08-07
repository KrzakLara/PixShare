using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Models
{
    public class PhotoThumbnail
    {
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime UploadDateTime { get; set; }
        public string Hashtags { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}

