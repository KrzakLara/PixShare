using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixShare.Factory
{
    public interface IPackageStrategy
    {
        void TrackConsumption(User user);
        void ChangePackage(User user, Package newPackage);
    }
}