using AveloMiddleware.Services;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AveloMiddleware.Helpers
{
    public class GenerateJWTToken
    {
        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        private readonly IConfiguration _config;
        public GenerateJWTToken(IConfiguration config)
        {
            // JWT specific initialization.
            _algorithm = new HMACSHA256Algorithm();
            _serializer = new JsonNetSerializer();
            _base64Encoder = new JwtBase64UrlEncoder();
            _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
            _config = config;
        }
        public async Task<string> IssuingJWT(string pnrNo, string lastName)
        {
            if (await isPnrNoAndLastNameValid(pnrNo, lastName))
            {
                Dictionary<string, object> claims = new Dictionary<string, object> {
                    {
                        "pnrNo",
                        pnrNo
                    },
                    {
                        "lastName",
                        lastName
                    },
                    {
                        "issuer",
                        "http://localhost:7154"
                    },
                    {
                        "aud",
                        "Read.All Write.All"
                    }
                };
                string token = _jwtEncoder.Encode(claims, _config.GetValue<string>("JWTSecretKey")); // Put this key in config
                return token;
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> isPnrNoAndLastNameValid(string pnr_no, string last_name)
        {
            var result = await ApiCallingService.CallAPI(
                        _config.GetValue<string>("SubscriptionKey"),
                        $"/reservation/v1.1/read/{pnr_no}/{last_name}",
                        new HttpMethod("GET"),
                        "CORE",
                        _config.GetValue<string>("CoreBaseUrl"),
                        "");
            if(result.StatusCode == StatusCodes.Status200OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
