using System;
using System.Web.Mvc;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Diagnostics;
using PixShare.Aspect;

namespace PixShare.Controllers
{
    public class LoginController : Controller
    {
        private IRepo _repo;

        public LoginController()
        {
            _repo = RepoFactory.GetRepo();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model state is valid.");
                Debug.WriteLine("Login attempt for email: " + user.Email);

                var authenticatedUser = _repo.AuthUser(user.Email, user.Password);

                if (authenticatedUser != null)
                {
                    // Store user information in session
                    Session["UserID"] = authenticatedUser.IDUser;
                    Session["Username"] = authenticatedUser.Username;
                    Session["UserType"] = authenticatedUser.UserType;

                    Debug.WriteLine("Login successful for user: " + authenticatedUser.Username);
                    return RedirectToAction("Index", "Home");
                }

                Debug.WriteLine("Login failed for email: " + user.Email);
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            else
            {
                Debug.WriteLine("Model state is invalid.");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Debug.WriteLine("Error: " + error.ErrorMessage);
                    }
                }
            }

            return View(user);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
