using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult ManageImages(int userId)
        {
            // Fetch the user to get the username
            var user = _repo.GetUserById(userId);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            // Fetch the last 10 uploaded posts by this user, ordered by date
            List<IPost> images = _repo.GetAllPosts()
                .Where(post => post.UserID == userId)
                .OrderByDescending(post => post.Date)
                .Take(10)
                .ToList();

            // Convert image paths to Base64 for display
            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ThumbnailPath))
                {
                    image.ThumbnailPath = ConvertImageToBase64(Server.MapPath(image.ThumbnailPath));
                }
                else if (!string.IsNullOrEmpty(image.ImagePath))
                {
                    image.ImagePath = ConvertImageToBase64(Server.MapPath(image.ImagePath));
                }
            }

            // Pass the username to the view
            ViewBag.Username = user.Username;

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
                return RedirectToAction("ManageImages", new { userId = post.UserID });
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
            return RedirectToAction("ManageImages", new { userId = post.UserID });
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

        private string ConvertImageToBase64(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                string extension = System.IO.Path.GetExtension(imagePath).TrimStart('.').ToLower();
                return $"data:image/{extension};base64,{base64ImageRepresentation}";
            }
            return null;
        }
    }
}
