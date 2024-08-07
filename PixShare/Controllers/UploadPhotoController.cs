using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixShare.Controllers
{
    public class UploadPhotoController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPhoto(HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                try
                {
                    // Specify the path where you want to save the uploaded photo.
                    string uploadPath = Server.MapPath("~/Uploads/");

                    // Create the directory if it doesn't exist.
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate a unique file name to prevent overwriting existing files.
                    string fileName = Path.Combine(uploadPath, Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));

                    // Save the file to the server.
                    imageFile.SaveAs(fileName);

                    // Optionally, you can save the file path or other information to a database.

                    // Redirect to a success page or perform other actions.
                    return RedirectToAction("UploadSuccess", "UploadPhoto", new { imagePath = fileName });


                }
                catch (Exception ex)
                {
                    // Handle exceptions, log errors, or redirect to an error page.
                    ViewBag.Error = "Error uploading the photo: " + ex.Message;
                    return View("UploadError");
                }
            }

            // Handle the case where no file was selected for upload.
            ViewBag.Error = "Please select a file to upload.";
            return View("UploadError");
        }

        public ActionResult UploadSuccess(string imagePath)
        {
            ViewBag.ImagePath = imagePath;
            return View();
        }


        public ActionResult UploadError()
        {
            // Display an error page.
            return View();
        }
    }

}