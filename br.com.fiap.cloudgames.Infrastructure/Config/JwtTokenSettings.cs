namespace br.com.fiap.cloudgames.Infrastructure.Config;

public class JwtTokenSettings
{
    public String Issuer { get; set; }
    public String Audience { get; set; }
    public String Key { get; set; }
    public int TokenTtlInMinutes { get; set; }
}