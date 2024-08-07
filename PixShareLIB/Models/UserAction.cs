using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Models
{
    public class UserAction
    {
        public int UserID { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
