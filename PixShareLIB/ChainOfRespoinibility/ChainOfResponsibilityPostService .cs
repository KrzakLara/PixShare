using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;

namespace PixShareLIB.ChainOfRespoinibility
{
    public class ChainOfResponsibilityPostService : IPostService
    {
        private readonly IRepo _repo;

        public ChainOfResponsibilityPostService(IRepo repo)
        {
            _repo = repo;
        }

        public List<IPost> GetAllPosts()
        {
            return _repo.GetAllPosts();
        }

        public void AddPost(IPost post)
        {
            _repo.AddPost(post);
        }

        public IPost GetPostById(int id)
        {
            return _repo.GetPostById(id);
        }

        public void UpdatePost(IPost post)
        {
            _repo.UpdatePost(post);
        }

        public List<IPost> GetFilteredPosts(FilterCriteria criteria)
        {
            var posts = _repo.GetAllPosts();

            var hashtagHandler = new HashtagFilterHandler();
            var sizeHandler = new SizeFilterHandler();
            var dateRangeHandler = new DateRangeFilterHandler();
            var authorHandler = new AuthorFilterHandler();

            // Ensure the handlers are set in the correct order
            authorHandler.SetNext(hashtagHandler).SetNext(sizeHandler).SetNext(dateRangeHandler);

            return authorHandler.Handle(posts, criteria).ToList();
        }
    }
}
