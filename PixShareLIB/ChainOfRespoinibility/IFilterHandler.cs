using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.ChainOfRespoinibility
{
    public interface IFilterHandler
    {
        IFilterHandler SetNext(IFilterHandler handler);
        IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria);
    }

}
