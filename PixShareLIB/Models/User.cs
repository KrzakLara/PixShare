using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using PixShare.State;
using PS_LIB.State;

namespace PixShareLIB.Models
{
    public class User
    {
        public int IDUser { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Username { get; set; }

        public string UserType { get; set; }

        public string PackageType { get; set; }

        public int CurrentConsumption { get; set; }
        public DateTime LastPackageChangeDate { get; set; }

        public IUserPackageState CurrentPackageState { get; set; }

        public User()
        {
            CurrentPackageState = new FreePackageState();
            LastPackageChangeDate = DateTime.MinValue;
        }

        public void SetPackageState(IUserPackageState newPackageState)
        {
            CurrentPackageState = newPackageState;
        }

        public void TrackConsumption()
        {
            CurrentPackageState.TrackConsumption(this);
        }
    }
}
