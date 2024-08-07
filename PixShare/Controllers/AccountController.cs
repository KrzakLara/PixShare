using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using PixShare.Strategy;
using System.Net;
using System.Web;

namespace PixShare.Controllers
{
    public class AccountController : Controller
    {
        private AuthContext _authContext;

      
    
        public async Task<ActionResult> ExternalLoginCallback()
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Your logic to handle successful login
            return RedirectToAction("Index", "Home");
        }
    }
}
