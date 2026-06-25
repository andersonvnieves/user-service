namespace br.com.fiap.cloudgames.Specs.ContextData;

public class UserAuthenticationContextData
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string GeneratedToken { get; set; }
    public Exception Exception { get; set; }
}