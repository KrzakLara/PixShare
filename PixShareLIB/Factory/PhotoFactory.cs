using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Factory
{
    public class PhotoFactory : IPhotoFactory
    {
        public PhotoThumbnail CreatePhotoThumbnail(IPost post, string author)
        {
            return new PhotoThumbnail
            {
                Description = post.Description,
                Author = author,
                UploadDateTime = post.Date,
                Hashtags = post.Hashtags,
                ThumbnailUrl = post.ImagePath
            };
        }
    }
}
