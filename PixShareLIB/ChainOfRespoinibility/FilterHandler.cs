using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.ChainOfRespoinibility
{
    public abstract class FilterHandler : IFilterHandler
    {
        private IFilterHandler _nextHandler;

        public IFilterHandler SetNext(IFilterHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual IEnumerable<IPost> Handle(IEnumerable<IPost> posts, FilterCriteria criteria)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(posts, criteria);
            }
            else
            {
                return posts;
            }
        }
    }

}
