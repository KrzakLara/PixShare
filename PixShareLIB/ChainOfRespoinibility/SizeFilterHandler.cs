using PixShareLIB.ChainOfRespoinibility;
using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;

public class SizeFilterHandler : FilterHandler
{
    public override IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria)
    {
        if (criteria.MinSize.HasValue)
        {
            posts = posts.Where(post => post.Size >= criteria.MinSize.Value);
        }
        if (criteria.MaxSize.HasValue)
        {
            posts = posts.Where(post => post.Size <= criteria.MaxSize.Value);
        }
        return base.Handle(posts, criteria);
    }
}
