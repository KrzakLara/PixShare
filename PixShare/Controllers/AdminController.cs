using System.Collections.Generic;
using System.Web.Mvc;
using PixShareLIB.DAL;
using PixShareLIB.Models;

namespace PixShare.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepo _repo;

        public AdminController()
        {
            _repo = RepoFactory.GetRepo();
        }

        public ActionResult Users()
        {
            var users = _repo.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = _repo.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdateUser(model);
                return RedirectToAction("Users"); // Redirect to the Users page
            }
            return View(model);
        }

        // User Statistics
        public ActionResult UserStatistics(int userId)
        {
            var actions = _repo.GetUserActions(userId);
            return View(actions);
        }

        // Manage Images
        public ActionResult ManageImages()
        {
            var images = _repo.GetAllPosts();
            return View(images);
        }

        // Edit Post
        [HttpGet]
        public ActionResult EditPost(int id)
        {
            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [HttpPost]
        public ActionResult EditPost(Post post)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdatePost(post);
                return RedirectToAction("ManageImages");
            }
            return View(post);
        }


        // Delete Post
        [HttpGet]
        public ActionResult DeleteImage(int id)
        {
            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            _repo.DeletePost(id);
            return RedirectToAction("ManageImages");
        }

        [HttpGet]
        public ActionResult DeletePostIndex(int id)
        {
            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            _repo.DeletePost(id);
            return RedirectToAction("Index", "Home");
        }
    }
}