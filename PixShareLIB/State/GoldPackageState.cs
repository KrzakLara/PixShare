using PixShare.State;
using PixShareLIB.Models;
using System;

namespace PixShareLIB.State
{
    public class GoldPackageState : IUserPackageState
    {
        public string PackageName => "GOLD";
        public int UploadSizeLimit => 10; // GB
        public int DailyUploadLimit => 100; // photos
        public int MaxPhotoConsumption => 1000; // MB

        public void TrackConsumption(User user)
        {
            if (user.CurrentConsumption < MaxPhotoConsumption)
            {
                user.CurrentConsumption += 100; // Example consumption increment
            }
            else
            {
                throw new Exception("Max photo consumption reached for the gold package.");
            }
        }

        public bool CanChangePackage(User user)
        {
            return (DateTime.Now - user.LastPackageChangeDate).TotalDays >= 1;
        }
    }
}
