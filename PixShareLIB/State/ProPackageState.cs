using PixShare.State;
using PixShareLIB.Models;
using System;

namespace PixShareLIB.State
{
    public class ProPackageState : IUserPackageState
    {
        public string PackageName => "PRO";
        public int UploadSizeLimit => 5; // GB
        public int DailyUploadLimit => 50; // photos
        public int MaxPhotoConsumption => 500; // MB

        public void TrackConsumption(User user)
        {
            if (user.CurrentConsumption < MaxPhotoConsumption)
            {
                user.CurrentConsumption += 50; // Example consumption increment
            }
            else
            {
                throw new Exception("Max photo consumption reached for the pro package.");
            }
        }

        public bool CanChangePackage(User user)
        {
            return (DateTime.Now - user.LastPackageChangeDate).TotalDays >= 1;
        }
    }
}
