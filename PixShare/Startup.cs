using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Google;
using Owin.Security.Providers.GitHub;
using Fluent.Infrastructure.FluentModel;
using Fluent.Infrastructure.FluentStartup;
using PixShare.Strategy;
using System;
using Microsoft.AspNet.Identity.Owin;

[assembly: OwinStartup(typeof(PixShare.Startup))]

namespace PixShare
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Use Google Auth Strategy
            var googleAuthContext = new AuthContext(new GoogleAuthStrategy());
            googleAuthContext.ConfigureAuth(app);

            // Use GitHub Auth Strategy
            var gitHubAuthContext = new AuthContext(new GitHubAuthStrategy());
            gitHubAuthContext.ConfigureAuth(app);
        }
    }
}
