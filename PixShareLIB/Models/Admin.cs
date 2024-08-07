using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Models
{
    public class Admin
    {
        public int AdminID { get; set; }
        public int UserID { get; set; }
        public bool ProfileManagment { get; set; }
        public bool UserActionsViewPremission { get; set; }
        public string AdminName { get; set; }
        public string AdminPassword { get; set; }

        public string Roles { get; set; }
    }
}
