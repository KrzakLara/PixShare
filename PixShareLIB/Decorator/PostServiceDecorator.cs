using PixShareLIB.Models;
using System.Collections.Generic;

public abstract class PostServiceDecorator : IPostService
{
    protected readonly IPostService _postService;

    protected PostServiceDecorator(IPostService postService)
    {
        _postService = postService;
    }

    public virtual List<IPost> GetAllPosts()
    {
        return _postService.GetAllPosts();
    }

    public virtual void UpdatePost(IPost post)
    {
        _postService.UpdatePost(post);
    }

    public virtual IPost GetPostById(int id) // Implement this method
    {
        return _postService.GetPostById(id);
    }
}
