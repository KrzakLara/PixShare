using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Factory
{
    public interface IPhotoFactory
    {
        PhotoThumbnail CreatePhotoThumbnail(IPost post, string author);
    }
}
