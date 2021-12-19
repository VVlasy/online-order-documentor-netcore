using Newtonsoft.Json;
using online_order_documentor_netcore.Models.PremierApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Providers
{
    public class PremierApiService
    {
        private static HttpClient _client = new HttpClient();

        static PremierApiService()
        {
            string authenticationString = $"{AppVariables.PremierUsername}:{AppVariables.PremierPassword}";
            string base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authenticationString));
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
        }

        private HttpRequestMessage Get_req(string content)
        {
            var r = new HttpRequestMessage();
            r.RequestUri = new Uri($"{AppVariables.PremierUrl}:{AppVariables.PremierPort}/api/comm");
            r.Method = HttpMethod.Post;

            r.Content = new StringContent(content);
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            {
                CharSet = Encoding.UTF8.WebName
            };
            r.Content.Headers.Add("ID-UJ", AppVariables.PremierIdUj);

            return r;
        }

        public static PremierApiVersion ApiVersion { get; set; }

        public PremierApiService()
        {
            if (ApiVersion == null)
            {
                var task = Task.Run(() => GetApiVersion());
                task.Wait();
                PremierApiVersion version = task.Result;

                ApiVersion = version;
            }
        }

        private async Task<PremierApiVersion> GetApiVersion()
        {
            PremierApiVersion version = null;
            var r = Get_req(JsonConvert.SerializeObject(new PremierRequest("VERZEAPI")));

            HttpResponseMessage response = await _client.SendAsync(r);

            if (response.IsSuccessStatusCode)
            {
                // Fix for wrong utf 8 calculation on Premier side

                var stream = await response.Content.ReadAsStreamAsync();

                StreamReader reader = new StreamReader(stream);
                string jsonString = reader.ReadToEnd();

                try
                {
                    PremierResponse result = JsonConvert.DeserializeObject<PremierResponse>(jsonString);
                    version = result.Data.ToObject<PremierApiVersion>();
                }
                catch 
                {

                }
            }

            return version;
        }
    }
}
