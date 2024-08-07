using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;

namespace PixShareLIB.ChainOfRespoinibility
{
    public class HashtagFilterHandler : FilterHandler
    {
        public override IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria)
        {
            if (!string.IsNullOrEmpty(criteria.Hashtags))
            {
                var hashtags = criteria.Hashtags.Split(' ').Select(h => h.Trim());
                posts = posts.Where(post => hashtags.Any(tag => post.Hashtags.Contains(tag)));
            }
            return base.Handle(posts, criteria);
        }
    }
}
