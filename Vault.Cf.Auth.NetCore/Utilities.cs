namespace Vault.Cf.Auth.NetCore
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Signers;
    using Org.BouncyCastle.OpenSsl;
    using System;
    using System.IO;
    using System.Text;

    internal class Utilities
    {
        private Utilities()
        {
        }

        public static string GetFileContents(string environmentVariable)
        {
            var path = Environment.GetEnvironmentVariable(environmentVariable);
            if (path == null)
            {
                Console.WriteLine($"({environmentVariable}) environment variable must be set.");
                return null;
            }

            var body = new StringBuilder();
            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    body.AppendLine(line);
                }

            }
            return body.ToString();
        }

        public static string GetStringToSign(string role, string signingTimeStr, string cfInstanceCert)
        {
            return $"{signingTimeStr}{cfInstanceCert}{role}";
        }

        public static string GenerateSignature(string privateKeyPem, byte[] data)
        {
            try
            {
                PemReader pemReader = new PemReader(new StringReader(privateKeyPem));
                var asymmetricCipherKeyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();

                var privateKey = asymmetricCipherKeyPair.Private as RsaPrivateCrtKeyParameters;

                var rsaKeyParameters = new RsaKeyParameters(true, privateKey.Modulus, privateKey.Exponent);

                PssSigner pssSigner = new PssSigner(new RsaEngine(), new Sha256Digest(), 222);
                pssSigner.Init(true, rsaKeyParameters);
                pssSigner.BlockUpdate(data, 0, data.Length);
                byte[] signature = pssSigner.GenerateSignature();
                
                return $"v1:{Convert.ToBase64String(signature)}";

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
