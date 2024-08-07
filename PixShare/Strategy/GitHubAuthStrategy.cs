using Owin.Security.Providers.GitHub;
using Owin;
using PixShare.Strategy;

public class GitHubAuthStrategy : IAuthStrategy
{
    public void Configure(IAppBuilder app)
    {
        app.UseGitHubAuthentication(new GitHubAuthenticationOptions
        {
            ClientId = "your-github-client-id",
            ClientSecret = "your-github-client-secret"
        });
    }
}
