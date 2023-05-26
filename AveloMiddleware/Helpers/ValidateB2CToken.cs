using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AveloMiddleware.Helpers
{
    public static class ValidateB2CToken
    {
        public static async Task<bool> Validate(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://dev-login.aveloair.com/4ad2f93c-9d89-433b-82e7-a0a884aa8242/v2.0/",
                ValidateAudience = true,
                ValidAudience = "8ba148af-e336-4f32-a321-7266e761c221",
                IssuerSigningKey = await GetSigningKeys()
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var user = handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                // Token is valid
                return true;
            }
            catch
            {
                // Token is invalid
                return false;
            }
        }

        private static async Task<RsaSecurityKey> GetSigningKeys()
        {
            var discoveryEndpoint = "https://aveloairdevb2c.b2clogin.com/aveloairdevb2c.onmicrosoft.com/b2c_1a_signup_signin/discovery/v2.0/keys";

            using (var httpClient = new HttpClient())
            {
                var discoveryResponse = await httpClient.GetAsync(discoveryEndpoint);
                dynamic discoveryDocument = await discoveryResponse.Content.ReadAsAsync<object>();

                var signingKeys = new List<SecurityKey>();

                RSA rsa = RSA.Create();
                foreach (var key in discoveryDocument?.keys)
                {
                    var exponent = GetBytesFromBase64Url(key?.e.ToString());
                    var modulus = GetBytesFromBase64Url(key?.n.ToString());
                    RSAParameters rsaParams = new RSAParameters
                    {
                        Exponent = exponent,
                        Modulus = modulus
                    };
                    rsa.ImportParameters(rsaParams);
                }

                return new RsaSecurityKey(rsa);
            }
        }

        private static byte[] GetBytesFromBase64Url(string element)
        {
            string base64Url = element;
            string base64 = Base64UrlDecode(base64Url);
            return Convert.FromBase64String(base64);
        }

        private static string Base64UrlDecode(string base64Url)
        {
            string base64 = base64Url;
            base64 = base64.Replace('_', '/').Replace('-', '+');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return base64;
        }
    }
}
