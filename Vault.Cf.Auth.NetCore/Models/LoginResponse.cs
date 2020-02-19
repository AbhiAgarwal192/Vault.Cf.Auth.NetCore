namespace Vault.Cf.Auth.NetCore.Models
{
    public class LoginResponse
    {
        public Auth auth { get; set; }
    }

    public class Auth
    {
        public string client_token { get; set; }
    }
}
