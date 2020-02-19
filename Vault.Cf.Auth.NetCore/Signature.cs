namespace Vault.Cf.Auth.NetCore
{
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using Vault.Cf.Auth.NetCore.Models;

    internal class Signature
    {
        private string _role;
        private string _signingTime;
        private string _cfInstanceCertContents;
        private string _signature;

        public Signature(string role)
        {
            string cfInstanceCert = Utilities.GetFileContents("CF_INSTANCE_CERT");
            string signingTimeStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string stringToSign = Utilities.GetStringToSign(role, signingTimeStr, cfInstanceCert);
            string cfInstanceKey = Utilities.GetFileContents("CF_INSTANCE_KEY");
            string signature = Utilities.GenerateSignature(cfInstanceKey, Encoding.ASCII.GetBytes(stringToSign));

            this._signingTime = signingTimeStr;
            this._role = role;
            this._cfInstanceCertContents = cfInstanceCert;
            this._signature = signature;
        }

        public string ToJsonString()
        {
            LoginRequest login = new LoginRequest
            {
                role = _role,
                signing_time = _signingTime,
                cf_instance_cert = _cfInstanceCertContents,
                signature = _signature
            };

            return JsonConvert.SerializeObject(login);
        }
    }
}
