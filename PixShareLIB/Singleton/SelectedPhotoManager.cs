using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Singleton
{
    public class SelectedPhotoManager
    {
        private static readonly Lazy<SelectedPhotoManager> instance = new Lazy<SelectedPhotoManager>(() => new SelectedPhotoManager());

        public static SelectedPhotoManager Instance => instance.Value;

        private SelectedPhotoManager() { }

        public IPost SelectedPhoto { get; private set; }

        public void SetSelectedPhoto(IPost photo)
        {
            SelectedPhoto = photo;
        }
    }
}
