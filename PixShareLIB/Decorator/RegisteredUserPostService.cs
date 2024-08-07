using PixShareLIB.Models;
using System.Web;
using System;

public class RegisteredUserPostService : PostServiceDecorator
{
    private readonly HttpSessionStateBase _session;

    public RegisteredUserPostService(IPostService postService, HttpSessionStateBase session)
        : base(postService)
    {
        _session = session;
    }

    public override void UpdatePost(IPost post)
    {
        if (_session["UserID"] == null || post.UserID != (int)_session["UserID"])
        {
            throw new UnauthorizedAccessException("You are not authorized to update this post.");
        }
        base.UpdatePost(post);
    }

    public override IPost GetPostById(int id) // Implement this method
    {
        return _postService.GetPostById(id);
    }
}
