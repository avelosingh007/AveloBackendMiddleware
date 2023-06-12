using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using Microsoft.Extensions.Configuration;
using AveloMiddleware.Services;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Internal;

namespace AveloMiddleware
{
    public static class AveloFunction
    {
        [FunctionName("Avelo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "Avelo")] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var type = req.Query["type"];

                if (!string.IsNullOrEmpty(type))
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);

                    Dictionary<string, string> baseUrlPairs = new Dictionary<string, string>();
                    baseUrlPairs.Add("AUTH", config.GetValue<string>("AuthBaseUrl"));
                    baseUrlPairs.Add("CORE", config.GetValue<string>("CoreBaseUrl"));
                    baseUrlPairs.Add("SLGNT", config.GetValue<string>("SelligentBaseUrl"));
                    baseUrlPairs.Add("WP", config.GetValue<string>("WpBaseUrl"));

                    var result = await ApiCallingService.CallAPI(
                        req.Query["api_route"],
                        new HttpMethod(req.Method),
                        type,
                        baseUrlPairs[type],
                        requestBody,
                        config);
                    return result;

                }
                else
                {
                    return new BadRequestObjectResult("Please pass type in the query string");
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }


        //This is not completed yet. This new fucntion is for handling all the authentication.
        [FunctionName("AveloAuth")]
        public static async Task<IActionResult> Auth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "Avelo/Auth")] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                return new OkObjectResult("Not completed yet");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
