using PixShareLIB.Models;

namespace PixShare.State
{
    public interface IUserPackageState
    {
        string PackageName { get; }
        int UploadSizeLimit { get; }
        int DailyUploadLimit { get; }
        int MaxPhotoConsumption { get; }
        void TrackConsumption(User user);
        bool CanChangePackage(User user);
    }
}
