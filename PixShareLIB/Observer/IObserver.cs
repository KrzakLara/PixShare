using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Observer
{
    public interface IObserver
    {
        void Update(string action, string user);
    }

}
