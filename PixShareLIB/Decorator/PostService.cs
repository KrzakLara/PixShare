using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Collections.Generic;

namespace PixShareLIB.Decorator
{
    public class PostService : IPostService
    {
        private readonly IRepo _repo;

        public PostService(IRepo repo)
        {
            _repo = repo;
        }

        public List<IPost> GetAllPosts()
        {
            return _repo.GetAllPosts();
        }

        public void UpdatePost(IPost post)
        {
            _repo.UpdatePost(post);
        }

        public IPost GetPostById(int id)
        {
            return _repo.GetPostById(id);
        }
    }
}
