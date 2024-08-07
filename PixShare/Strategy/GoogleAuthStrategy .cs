using Microsoft.Owin.Security.Google;
using Owin;
using PixShare.Strategy;

public class GoogleAuthStrategy : IAuthStrategy
{
    public void Configure(IAppBuilder app)
    {
        app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
        {
            ClientId = "your-google-client-id",
            ClientSecret = "your-google-client-secret"
        });
    }
}
