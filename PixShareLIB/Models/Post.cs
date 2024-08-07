using PixShareLIB.Models;
using System;

namespace PixShareLIB.Models
{
    public class Post : IPost
{
    public int IDPost { get; set; }
    public string Description { get; set; }
    public string Hashtags { get; set; }
    public string ImagePath { get; set; }
    public string ThumbnailPath { get; set; }
    public DateTime Date { get; set; }
    public int UserID { get; set; }
    public long? Size { get; set; }
    public string Username { get; set; } 
}
}