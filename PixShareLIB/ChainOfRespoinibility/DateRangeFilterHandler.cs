using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;

namespace PixShareLIB.ChainOfRespoinibility
{
    public class DateRangeFilterHandler : FilterHandler
    {
        public override IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria)
        {
            if (criteria.StartDate.HasValue)
            {
                posts = posts.Where(post => post.Date >= criteria.StartDate.Value);
            }
            if (criteria.EndDate.HasValue)
            {
                posts = posts.Where(post => post.Date <= criteria.EndDate.Value);
            }
            return base.Handle(posts, criteria);
        }
    }
}
