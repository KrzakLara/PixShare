using System;

namespace PixShareLIB.Models
{
    public interface IPost
    {
        int IDPost { get; set; }
        string Description { get; set; }
        string Hashtags { get; set; }
        string ImagePath { get; set; }
        string ThumbnailPath { get; set; }
        DateTime Date { get; set; }
        int UserID { get; set; }
        long? Size { get; set; }
        string Username { get; set; } // Add this property
    }
}
