using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixShare.Strategy
{
    public interface IAuthStrategy
    {
        void Configure(IAppBuilder app);
    }


}