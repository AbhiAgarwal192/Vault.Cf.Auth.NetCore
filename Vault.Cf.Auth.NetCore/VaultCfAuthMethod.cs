namespace Vault.Cf.Auth.NetCore
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using Vault.Cf.Auth.NetCore.Models;

    public class VaultCfAuthMethod
    {
        private string _token;

        public VaultCfAuthMethod(string vaultAddress, string vaultNamespace, string vaultRole)
        {
            string cfAuthContext = "v1/auth/cf/login";
            Signature signature = new Signature(vaultRole);

            string body = signature.ToJsonString();

            try
            {
                using (var client = new HttpClient())
                {
                    _ = Uri.TryCreate(vaultAddress, UriKind.Absolute, out Uri uri);
                    client.BaseAddress = uri;
                    var stringContent = new StringContent(body, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("X-Vault-Namespace", vaultNamespace);

                    var httpResponseMessage = client.PostAsync(cfAuthContext, stringContent).Result;

                    var responseData = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);

                        _token = loginResponse.auth.client_token;
                    }
                    else
                    {
                        Console.WriteLine($"StatusCode: {httpResponseMessage.StatusCode}  ReasonPhrase: {httpResponseMessage.ReasonPhrase} ResponseData: {responseData}");
                        _token = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetToken()
        {
            return this._token;
        }
    }
}
