using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string Title { get; set; }
        public int UploadSize { get; set; }
        public int DailyUpload { get; set; }
        public int MaxPhotos { get; set; }
        public decimal Price { get; set; }
        public int UserID { get; set; }
    }
}
