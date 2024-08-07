using PixShareLIB.Models;
using System.Collections.Generic;

public interface IPostService
{
    List<IPost> GetAllPosts();
    void UpdatePost(IPost post);
    IPost GetPostById(int id); // Add this method
}
