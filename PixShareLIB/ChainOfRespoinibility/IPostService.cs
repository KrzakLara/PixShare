using PixShareLIB.Models;
using System.Collections.Generic;

namespace PixShareLIB.ChainOfRespoinibility
{
    public interface IPostService
    {
        List<IPost> GetAllPosts();
        void AddPost(IPost post);
        IPost GetPostById(int id);
        void UpdatePost(IPost post);
        List<IPost> GetFilteredPosts(FilterCriteria criteria);
    }
}
