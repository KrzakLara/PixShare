using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;

namespace PixShareLIB.ChainOfRespoinibility
{
    public class AuthorFilterHandler : FilterHandler
    {
        public override IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria)
        {
            if (!string.IsNullOrEmpty(criteria.Username))
            {
                posts = posts.Where(post => post.Username == criteria.Username);
            }
            return base.Handle(posts, criteria);
        }
    }

}
