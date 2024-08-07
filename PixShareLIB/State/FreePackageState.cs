using PixShare.State;
using PixShareLIB.Models;
using System;

namespace PS_LIB.State
{
    public class FreePackageState : IUserPackageState
    {
        public string PackageName => "FREE";
        public int UploadSizeLimit => 1; // GB
        public int DailyUploadLimit => 10; // photos
        public int MaxPhotoConsumption => 100; // MB

        public void TrackConsumption(User user)
        {
            const int photoSize = 10; // MB

            if (user.CurrentConsumption + photoSize > MaxPhotoConsumption)
            {
                throw new InvalidOperationException("Exceeded maximum photo consumption for the free package.");
            }

            user.CurrentConsumption += photoSize;
        }

        public bool CanChangePackage(User user)
        {
            return (DateTime.Now - user.LastPackageChangeDate).TotalDays >= 1;
        }
    }
}
