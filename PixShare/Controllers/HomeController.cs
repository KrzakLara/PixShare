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
        private readonly UserActions _userActions;

        public HomeController() : this(new DBRepo(), new HttpContextWrapper(System.Web.HttpContext.Current).Session)
        {
        }

        public HomeController(IRepo repo, HttpSessionStateBase session)
        {
            _repo = repo;
            _postService = new ChainOfResponsibilityPostService(_repo);

            // Initialize UserActions and attach the logger
            _userActions = new UserActions();
            _userActions.Attach(new Logger());
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult Index()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Index", userName);

            // Get the last 10 uploaded photos
            List<IPost> photos = _postService.GetAllPosts()
                .OrderByDescending(post => post.Date)
                .Take(10)
                .ToList();

            // Convert image paths to Base64
            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo.ThumbnailPath))
                {
                    photo.ThumbnailPath = ConvertImageToBase64(Server.MapPath(photo.ThumbnailPath));
                }
                else if (!string.IsNullOrEmpty(photo.ImagePath))
                {
                    photo.ImagePath = ConvertImageToBase64(Server.MapPath(photo.ImagePath));
                }
            }

            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult About()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View About", userName);

            ViewBag.Message = "Your application description page.";
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Contact()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Contact", userName);

            ViewBag.Message = "Your contact page.";
            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Logout()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("Logout", userName);

            Session.Clear();
            return RedirectToAction("HomeScreen");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Registration()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Registration", userName);

            return View("~/Views/Registration/Index.cshtml");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult Anonymous()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("Anonymous", userName);

            return RedirectToAction("Index");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Packages()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Packages", userName);

            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult HomeScreen()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View HomeScreen", userName);

            // Get the last 10 uploaded photos
            List<IPost> photos = _postService.GetAllPosts()
                .OrderByDescending(post => post.Date)
                .Take(10)
                .ToList();

            // Convert image paths to Base64
            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo.ThumbnailPath))
                {
                    photo.ThumbnailPath = ConvertImageToBase64(Server.MapPath(photo.ThumbnailPath));
                }
                else if (!string.IsNullOrEmpty(photo.ImagePath))
                {
                    photo.ImagePath = ConvertImageToBase64(Server.MapPath(photo.ImagePath));
                }
            }

            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult PackageConsumption()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View PackageConsumption", userName);

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
            return View(user);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult ChangePackage(string packageType)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction($"Change Package to {packageType}", userName);

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserID"];
            var user = _repo.GetUserById(userId);

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
            user.LastPackageChangeDate = DateTime.Now;
            _repo.UpdateUser(user);

            return RedirectToAction("PackageConsumption");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult UploadPost()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View UploadPost", userName);

            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult UploadPost(HttpPostedFileBase file, Post photo, string processingType, int? width, int? height)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";

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

                switch (processingType)
                {
                    case "Resize":
                        if (width.HasValue && height.HasValue)
                        {
                            ResizeImage(originalPath, thumbnailPath, width.Value, height.Value);
                        }
                        break;
                    case "PNG":
                        ConvertImageFormat(originalPath, thumbnailPath, ImageFormat.Png);
                        break;
                    case "JPG":
                        ConvertImageFormat(originalPath, thumbnailPath, ImageFormat.Jpeg);
                        break;
                    case "BMP":
                        ConvertImageFormat(originalPath, thumbnailPath, ImageFormat.Bmp);
                        break;
                    default:
                        thumbnailPath = originalPath; // If no processing, keep the original image
                        break;
                }

                photo.ImagePath = "/UploadedImages/" + Path.GetFileName(file.FileName);
                photo.ThumbnailPath = "/UploadedImages/Thumbnails/" + Path.GetFileName(file.FileName);
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
                    _userActions.PerformAction("Upload Photo", userName);
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
            ProcessImage(originalImagePath, outputPath, (img, outPath) =>
            {
                var thumbnail = img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                thumbnail.Save(outPath);
            });
        }

        private void ConvertImageFormat(string originalImagePath, string outputPath, ImageFormat format)
        {
            ProcessImage(originalImagePath, outputPath, (img, outPath) => img.Save(outPath, format));
        }

        private void ProcessImage(string originalImagePath, string outputPath, Action<System.Drawing.Image, string> processAction)
        {
            using (var image = System.Drawing.Image.FromFile(originalImagePath))
            {
                processAction(image, outputPath);
            }
        }

        private string ConvertImageToBase64(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                string extension = Path.GetExtension(imagePath).TrimStart('.').ToLower();
                return $"data:image/{extension};base64,{base64ImageRepresentation}";
            }
            return null;
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult UploadSuccess()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View UploadSuccess", userName);

            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        public ActionResult Explore()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Explore", userName);

            // Get the last 10 uploaded photos
            List<IPost> photos = _postService.GetAllPosts()
                .OrderByDescending(post => post.Date)
                .Take(10)
                .ToList();

            // Convert image paths to Base64
            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo.ThumbnailPath))
                {
                    photo.ThumbnailPath = ConvertImageToBase64(Server.MapPath(photo.ThumbnailPath));
                }
                else if (!string.IsNullOrEmpty(photo.ImagePath))
                {
                    photo.ImagePath = ConvertImageToBase64(Server.MapPath(photo.ImagePath));
                }
            }

            return View(photos);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult SavePhoto(Post model)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";

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
                        _userActions.PerformAction("Save Photo", userName);
                        return RedirectToAction("Index");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                        return View(model);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult EditPost(int id)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View EditPost", userName);

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
            var userName = Session["Username"]?.ToString() ?? "Unknown User";

            return ProcessAndHandlePost(photo, p =>
            {
                _postService.UpdatePost(p);
                _userActions.PerformAction("Edit Post", userName);
                return RedirectToAction("Index");
            }, ex =>
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(photo);
            });
        }

        private ActionResult ProcessAndHandlePost(Post post, Func<Post, ActionResult> successAction, Func<Exception, ActionResult> errorAction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return successAction(post);
                }
                catch (Exception ex)
                {
                    return errorAction(ex);
                }
            }
            return RedirectToAction("Index");
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult ShowPhoto(int id)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View ShowPhoto", userName);

            var post = _postService.GetPostById(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrEmpty(post.ImagePath))
            {
                ViewBag.ErrorMessage = "Image path is not available.";
                return View("Error"); // Or handle this case as needed
            }

            // Convert image to Base64
            string base64Image = ConvertImageToBase64(Server.MapPath(post.ImagePath));
            ViewBag.Base64Image = base64Image; // Pass the Base64 string to the view
            return View(post);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult Search()
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View Search", userName);

            return View();
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpPost]
        public ActionResult Search(FilterCriteria criteria)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("Search", userName);

            var posts = _postService.GetFilteredPosts(criteria);
            return View("SearchResults", posts);
        }

        [LoggingAspect]
        [ExceptionHandlingAspect]
        [HttpGet]
        public ActionResult DownloadPhoto(int id)
        {
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("View DownloadPhoto", userName);

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
            var userName = Session["Username"]?.ToString() ?? "Unknown User";
            _userActions.PerformAction("Download Photo", userName);

            var post = _postService.GetPostById(model.PostId);
            if (post == null)
            {
                return HttpNotFound();
            }

            return TransformAndDownloadPhoto(post.ImagePath, model, (outputPath) =>
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(outputPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(outputPath));
            });
        }

        private ActionResult TransformAndDownloadPhoto(string imagePath, DownloadPhotoViewModel model, Func<string, ActionResult> downloadAction)
        {
            string originalPath = Server.MapPath(imagePath);
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

            ImageFormat imageFormat = ConvertStringToImageFormat(model.Format);
            if (imageFormat != null)
            {
                builder.ConvertToFormat(imageFormat);
            }

            string downloadedPath = builder.Save(outputDirectory);
            return downloadAction(downloadedPath);
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
