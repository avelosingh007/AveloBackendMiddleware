using AveloMiddleware.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AveloMiddleware.Services
{
    public static class ApiCallingService
    {

        public static async Task<ObjectResult> CallAPI(string apiRoute, HttpMethod httpMethod, string type, string baseUrl, string body, IConfigurationRoot config)
        {
            HttpClient client = new HttpClient();
            string result = string.Empty;
            var content = new StringContent(body, null, "application/json");
            HttpResponseMessage response = new HttpResponseMessage();


            var request = new HttpRequestMessage(httpMethod, $"{baseUrl}/{apiRoute}");
            switch (type)
            {
                case "AUTH":
                    break;
                case "CORE":
                    request.Headers.Add("Ocp-Apim-Subscription-Key", config.GetValue<string>("SubscriptionKey"));
                    request.Content = content;
                    response = await client.SendAsync(request);
                    result = await response.Content?.ReadAsStringAsync();

                    break;
                case "SLGNT":
                    request.Headers.Add("X-ApiKey", config.GetValue<string>("XApiKey"));
                    request.Content = content;
                    response = await client.SendAsync(request);
                    result = await response.Content?.ReadAsStringAsync();

                    break;
                case "WP":
                    request.Headers.Add("Authorization", config.GetValue<string>("WPApplicationPassword"));
                    request.Content = content;
                    response = await client.SendAsync(request);
                    result = await response.Content?.ReadAsStringAsync();

                    break;
                default:
                    return new BadRequestObjectResult("No api called");
            }

            return new ActionResultMapper(JObject.Parse(result), ((int)response.StatusCode));
        }
    }
}
