using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Converters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueCardPortal.Infrastructure.Integrations.BlueCardCore
{
    partial class Client
    {
        private readonly string? jsonDir;
        private readonly string? userName;
        private readonly string? password;
        private readonly IMemoryCache cache;
        private readonly string loginUrl;
        private readonly ILogger logger;

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url) {
            //string? token;

            //if (cache.TryGetValue(CashConstants.Token, out token))
            //{
            //    request.Headers.Add("Authorization", "Bearer " + token);
            //}
            //else
            //{
            //    string header = GetAutorizationHeader(client);

            //    request.Headers.Add("Authorization", header);
            //}

            string? body = request?.Content?.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(jsonDir))
            {
                var fileName = DateTime.Now.ToString("yyyy-MM-dd_hh_mm_ss_fff")+ ".json";
                File.WriteAllText(Path.Combine(jsonDir, fileName), body);
            }
            logger.LogDebug($"Request to {url} with body: {body}");

            string header = GetAutorizationHeader(client);

            request.Headers.Add("Authorization", header);
        }

        partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
            //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //{
            //    cache.Remove(CashConstants.Token);
            //}            
        }

        static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
        {
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }

        private string GetAutorizationHeader(HttpClient client)
        {
            var authenticationString = $"{userName}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            var request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            string header = $"Basic {base64EncodedAuthenticationString}";
            request.Headers.Add("Authorization", header);
            
            var jsonData = JsonConvert.SerializeObject(new 
            {
                refresh_groups = true,
                requested_lifetime = 7200
            });

            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            BlueCardToken? token = null;

            try
            {
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    token = JsonConvert.DeserializeObject<BlueCardToken>(responseContent);
                }
            }
            catch (Exception)
            {}

            if (token != null)
            {
                cache.Set(CashConstants.Token, token.csrf_token, TimeSpan.FromSeconds(token.expiration - 5));
                header = $"Bearer {token.csrf_token}";
            }
            
            return header;
        }

        // Конструктора да се изтрие от генерирания клиент
        public Client(
            IHttpClientFactory clientFactory, 
            IConfiguration config, 
            IMemoryCache _cache, 
            ILogger<Client> _logger)
        {
            jsonDir = config.GetValue<string>("Integration:JsonDir");
            userName = config.GetValue<string>("Integration:User");
            password = config.GetValue<string>("Integration:Password");
            loginUrl = config.GetValue<string>("Integration:LoginUrl") ?? string.Empty;
            _baseUrl = config.GetValue<string>("Integration:BaseUrl") ?? string.Empty;

            if (_baseUrl.EndsWith("/") == false)
            {
                _baseUrl += "/";
            }

            bool ignoreSSl = config.GetValue<bool>("Integration:IgnoreSSLErrors");
            if (ignoreSSl)
            {
                _httpClient = clientFactory.CreateClient("insecureClient");
            }
            else
            {
                _httpClient = clientFactory.CreateClient();
            }

            cache = _cache;
            logger = _logger;
        }
    }
}
