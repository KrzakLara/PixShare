using PixShareLIB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixShare.Factory
{
    public class TrackConsumptionStrategy : IPackageStrategy
    {
        public void TrackConsumption(User user)
        {
            // Implement consumption tracking logic
            // Update user's consumption attributes based on package
        }

        public void ChangePackage(User user, Package newPackage)
        {
            // Implement logic to change the user's package
            // Ensure that users can change their package once a day and apply the change from the following day
        }
    }
}