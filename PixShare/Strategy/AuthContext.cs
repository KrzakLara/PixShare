using Owin;
using PixShare.Strategy;

public class AuthContext
{
    private readonly IAuthStrategy _authStrategy;

    public AuthContext(IAuthStrategy authStrategy)
    {
        _authStrategy = authStrategy;
    }

    public void ConfigureAuth(IAppBuilder app)
    {
        _authStrategy.Configure(app);
    }
}
