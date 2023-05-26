using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AveloMiddleware.Helpers
{
    public class ValidateJWT
    {
        public bool IsValid { get; } = false;
        public string pnrNo { get; }
        public string lastName { get; set; }

        public ValidateJWT(HttpRequest request)
        {
            // Check if we have a header.
            if (!request.Headers.ContainsKey("Authorization"))
            {
                IsValid = false;
                return;
            }
            string authorizationHeader = request.Headers["Authorization"];
            // Check if the value is empty.
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                IsValid = false;
                return;
            }
            // Check if we can decode the header.
            IDictionary<string, object> claims = null;
            try
            {
                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }
                // Validate the token and decode the claims.
                claims = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret("Your Secret Securtity key string").MustVerifySignature().Decode<IDictionary<string, object>>(authorizationHeader);
            }
            catch (Exception exception)
            {
                IsValid = false;
                return;
            }
        }
    }
}
