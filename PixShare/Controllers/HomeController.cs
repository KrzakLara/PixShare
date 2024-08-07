using System.Web.Mvc;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Collections.Generic;
using System.Linq;
using PixShare.Aspect;
using PixShare.State;
using PixShareLIB.Builder;
using PixShareLIB.ChainOfRespoinibility;
using PixShareLIB.Observer;
using PixShareLIB.State;
using PS_LIB.State;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System;

namespace PixShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepo _repo;
        private readonly PixShareLIB.ChainOfRespoinibility.IPostService _postService;
        private readonly UserActions _userActions = new UserActions();

        public HomeController() : this(new DBRepo(), new HttpContextWrapper(System.Web.HttpContext.Current).Session)
        {
        }

        public HomeController(IRepo repo, HttpSessionStateBase session)
        {
            _repo = repo;
            _postService = new ChainOfResponsibilityPostService(_repo);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult Index()
        {
            List<IPost> photos = _postService.GetAllPosts().Take(10).ToList();
            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("HomeScreen");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Registration()
        {
            return View("~/Views/Registration/Index.cshtml");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult Anonymous()
        {
            return RedirectToAction("Index");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Packages()
        {
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult HomeScreen()
        {
            List<IPost> photos = _postService.GetAllPosts().Take(10).ToList();
            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult PackageConsumption()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "HomeController");
            }

            int userId = (int)Session["UserID"];
            var user = _repo.GetUserById(userId);

            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            if (user.CurrentPackageState == null)
            {
                user.CurrentPackageState = new FreePackageState();
            }

            user.TrackConsumption();
            _userActions.PerformAction("View PackageConsumption", user.Username);
            return View(user);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult ChangePackage(string packageType)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserID"];
            var user = _repo.GetUserById(userId);

            // Check if the user can change the package
            if (!user.CurrentPackageState.CanChangePackage(user))
            {
                TempData["ErrorMessage"] = "You can change your package again after 24 hours from the last change.";
                return RedirectToAction("PackageConsumption");
            }

            IUserPackageState newPackageState;
            switch (packageType)
            {
                case "PRO":
                    newPackageState = new ProPackageState();
                    break;
                case "GOLD":
                    newPackageState = new GoldPackageState();
                    break;
                default:
                    newPackageState = new FreePackageState();
                    break;
            }

            user.SetPackageState(newPackageState);
            user.LastPackageChangeDate = DateTime.Now; // Update the last package change date
            _repo.UpdateUser(user);
            _userActions.PerformAction("Change Package to " + packageType, user.Username);

            return RedirectToAction("PackageConsumption");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult UploadPhoto()
        {
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult UploadPost(HttpPostedFileBase file, Post photo, string processingType, int? width, int? height)
        {
            if (file != null && file.ContentLength > 0)
            {
                string originalPath = Path.Combine(Server.MapPath("~/UploadedImages"), Path.GetFileName(file.FileName));
                string thumbnailDir = Server.MapPath("~/UploadedImages/Thumbnails");
                string thumbnailPath = Path.Combine(thumbnailDir, Path.GetFileName(file.FileName));

                if (!Directory.Exists(thumbnailDir))
                {
                    Directory.CreateDirectory(thumbnailDir);
                }

                file.SaveAs(originalPath);

                // Get file size
                photo.Size = new FileInfo(originalPath).Length;

                // Process the image based on the user's choice
                switch (processingType)
                {
                    case "Resize":
                        if (width.HasValue && height.HasValue)
                        {
                            ResizeImage(originalPath, thumbnailPath, width.Value, height.Value);
                        }
                        break;
                    case "PNG":
                        ConvertImageFormat(originalPath, thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case "JPG":
                        ConvertImageFormat(originalPath, thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "BMP":
                        ConvertImageFormat(originalPath, thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }

                photo.ImagePath = "/UploadedImages/" + Path.GetFileName(file.FileName);
                photo.Date = DateTime.Now;

                if (Session["UserID"] != null)
                {
                    photo.UserID = (int)Session["UserID"];
                }
                else
                {
                    ViewBag.ErrorMessage = "User not logged in or session expired.";
                    return View(photo);
                }

                try
                {
                    _repo.AddPost(photo);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(photo);
                }
            }
            return View(photo);
        }

        private void ResizeImage(string originalImagePath, string outputPath, int width, int height)
        {
            using (var image = System.Drawing.Image.FromFile(originalImagePath))
            {
                var thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                thumbnail.Save(outputPath);
            }
        }

        private void ConvertImageFormat(string originalImagePath, string outputPath, System.Drawing.Imaging.ImageFormat format)
        {
            using (var image = System.Drawing.Image.FromFile(originalImagePath))
            {
                image.Save(outputPath, format);
            }
        }

        private void CreateThumbnail(string originalImagePath, string thumbnailPath)
        {
            try
            {
                using (var image = System.Drawing.Image.FromFile(originalImagePath))
                {
                    var thumbnail = image.GetThumbnailImage(150, 150, () => false, IntPtr.Zero);

                    // Ensure the directory exists before saving
                    string directory = Path.GetDirectoryName(thumbnailPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Save the thumbnail to the correct path
                    thumbnail.Save(thumbnailPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create thumbnail. " + ex.Message);
            }
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult UploadSuccess()
        {
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Explore()
        {
            List<IPost> photos = _postService.GetAllPosts();
            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult SavePhoto(Post model)
        {
            if (ModelState.IsValid)
            {
                var photo = _repo.GetPostById(model.IDPost);
                if (photo != null)
                {
                    photo.Description = model.Description;
                    photo.Hashtags = model.Hashtags;
                    try
                    {
                        _postService.UpdatePost(photo);
                        return RedirectToAction("Index"); // Redirect to the main view after saving
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                        return View(model); // Return the same view with the error message
                    }
                }
            }

            return RedirectToAction("Index"); // Return to Index if model state is invalid
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult EditPost(int id)
        {
            var photo = _postService.GetPostById(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult EditPost(Post photo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postService.UpdatePost(photo);
                    return RedirectToAction("Index");
                }
                catch (UnauthorizedAccessException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }
            return View(photo);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult ShowPhoto(int id)
        {
            var post = _postService.GetPostById(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.PhotoPath = post.ImagePath;
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult Search(FilterCriteria criteria)
        {
            var posts = _postService.GetFilteredPosts(criteria);
            return View("SearchResults", posts);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult DownloadPhoto(int id)
        {
            var model = new DownloadPhotoViewModel
            {
                PostId = id
            };

            ViewBag.FormatOptions = new SelectList(new[]
            {
                new SelectListItem { Value = "jpeg", Text = "JPEG" },
                new SelectListItem { Value = "png", Text = "PNG" },
                new SelectListItem { Value = "bmp", Text = "BMP" }
            }, "Value", "Text");

            return View(model);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult DownloadPhoto(DownloadPhotoViewModel model)
        {
            var post = _postService.GetPostById(model.PostId);
            if (post == null)
            {
                return HttpNotFound();
            }

            string originalPath = Server.MapPath(post.ImagePath);
            string outputDirectory = Server.MapPath("~/DownloadedImages");

            var builder = new PhotoBuilder(originalPath);

            if (model.ApplyResize)
            {
                builder.Resize(model.Width, model.Height);
            }

            if (model.ApplySepia)
            {
                builder.ApplySepia();
            }

            if (model.ApplyBlur)
            {
                builder.ApplyBlur();
            }

            // Convert string format to ImageFormat
            ImageFormat imageFormat = ConvertStringToImageFormat(model.Format);
            if (imageFormat != null)
            {
                builder.ConvertToFormat(imageFormat);
            }

            string downloadedPath = builder.Save(outputDirectory);
            byte[] fileBytes = System.IO.File.ReadAllBytes(downloadedPath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(downloadedPath));
        }

        private ImageFormat ConvertStringToImageFormat(string format)
        {
            switch (format.ToLower())
            {
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "png":
                    return ImageFormat.Png;
                case "bmp":
                    return ImageFormat.Bmp;
                default:
                    return null;
            }
        }
    }
}
