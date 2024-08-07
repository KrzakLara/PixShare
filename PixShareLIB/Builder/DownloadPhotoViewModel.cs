using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace PixShareLIB.Builder
{
    public class DownloadPhotoViewModel
    {
        public int PostId { get; set; }
        public bool ApplyResize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool ApplySepia { get; set; }
        public bool ApplyBlur { get; set; }
        public string Format { get; set; }  // Use string instead of ImageFormat
    }
}
