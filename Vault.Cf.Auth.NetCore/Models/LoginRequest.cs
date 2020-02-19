namespace Vault.Cf.Auth.NetCore.Models
{
    public class LoginRequest
    {
        public string role { get; set; }

        public string signing_time { get; set; }

        public string cf_instance_cert { get; set; }

        public string signature { get; set; }
    }
}
