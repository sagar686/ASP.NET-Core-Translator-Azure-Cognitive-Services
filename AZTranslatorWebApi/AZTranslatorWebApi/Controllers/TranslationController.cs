using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using AZTranslatorWebApi.Models;
using Microsoft.Extensions.Configuration;

namespace AZTranslatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {

        private readonly IConfiguration _config;

        public TranslationController(IConfiguration config)
        {
            _config = config;
        }

        [Route("GetSupportedLanguages")]
        [HttpGet]
        public async Task<ActionResult<string>> GetSupportedLanguages()
        {
            string endpoint = _config.GetValue<string>("AZTextTranslator:Settings:SupportedLanguagesEndpoint");
            var client = new HttpClient();
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint);
                var response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();               
                
                return result;
            }
        }

        [HttpPost]        
        public async Task<ActionResult<string>> Post(TranslationInput input)
        {
            TranslationResult[] translationResultList = await TranslateText(input.Text, input.TargetLanguage);

            return Newtonsoft.Json.JsonConvert.SerializeObject(translationResultList);
        }
        async Task<TranslationResult[]> TranslateText(string inputText, string targetLanguage)
        {
            string subscriptionKey = _config.GetValue<string>("AZTextTranslator:Settings:SubscriptionKey");
            string region = _config.GetValue<string>("AZTextTranslator:Settings:Region"); 
            string translateTextEndpoint = _config.GetValue<string>("AZTextTranslator:Settings:Endpoint"); 
            string route = $"/translate?api-version=3.0&to={targetLanguage}";
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(translateTextEndpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);

                HttpResponseMessage responseMessage = await client.SendAsync(request).ConfigureAwait(false);
                string result = await responseMessage.Content.ReadAsStringAsync();
                TranslationResult[] translationResult = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                return translationResult;
            }
        }

    }
}
