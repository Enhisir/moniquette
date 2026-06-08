namespace Moniquette.Server.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = "Moniquette.Server";

    public string Audience { get; set; } = "Moniquette.Client";

    public string Secret { get; set; } = string.Empty;

    public int TokenLifetimeMinutes { get; set; } = 240;
}
