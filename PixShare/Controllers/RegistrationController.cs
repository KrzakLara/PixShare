using System.Web.Mvc;
using PixShareLIB.DAL;
using PixShareLIB.Models;

namespace PixShare.Controllers
{
    public class RegistrationController : Controller
    {
        private IRepo _repo;

        public RegistrationController()
        {
            _repo = RepoFactory.GetRepo();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(); // Ensure this returns the Index view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserType = "User"; // Ensure the UserType is set to "User" by default
                _repo.CreateUser(user);
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }
    }
}
