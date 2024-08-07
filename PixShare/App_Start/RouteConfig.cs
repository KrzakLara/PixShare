using System.Web.Mvc;
using System.Web.Routing;

namespace PixShare
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
                name: "PasswordForgotten",
                url: "Home/PasswordForgotten",
                defaults: new { controller = "Home", action = "PasswordForgotten" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
                name: "Register",
                url: "Registration/Register",
                defaults: new { controller = "Registration", action = "RegisterUser" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
                name: "Packages",
                url: "Home/Packages",
                defaults: new { controller = "Home", action = "Packages" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
                name: "HomeScreen",
                url: "Home/HomeScreen",
                defaults: new { controller = "Home", action = "HomeScreen" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
                name: "UserLogin",
                url: "Login/UserLogin",
                defaults: new { controller = "Login", action = "UserLogin" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
             name: "PackageConsumption",
             url: "Home/PackageConsumption",
             defaults: new { controller = "Home", action = "PackageConsumption" },
             namespaces: new[] { "PixShare.Controllers" }
         );



            routes.MapRoute(
                name: "UploadPhoto",
                url: "Home/UploadPhoto",
                defaults: new { controller = "Home", action = "UploadPhoto" },
                namespaces: new[] { "PixShare.Controllers" }
            );


            routes.MapRoute(
                name: "HashPasswords",
                url: "Admin/HashPasswords",
                defaults: new { controller = "Admin", action = "HashPasswords" },
                namespaces: new[] { "PixShare.Controllers" }
            );

            routes.MapRoute(
            name: "DownloadPhoto",
            url: "Home/DownloadPhoto/{id}",
            defaults: new { controller = "Home", action = "DownloadPhoto", id = UrlParameter.Optional }
        );
            routes.MapRoute(
                        name: "UserStatistics",
                        url: "Admin/UserStatistics/{userId}",
                        defaults: new { controller = "Admin", action = "UserStatistics", userId = UrlParameter.Optional }
                    );

            routes.MapRoute(
                name: "ManageImages",
                url: "Admin/ManageImages/{userId}",
                defaults: new { controller = "Admin", action = "ManageImages", userId = UrlParameter.Optional }
            );
        }
    }
}