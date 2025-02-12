using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WashMachine.Forms.Common.Http
{
    public class HttpService : IHttpService
    {
        public T ConvertTo<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return default(T);
        }

        public async Task<string> Get(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string requestUri = url;
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                catch (HttpRequestException e)
                {
                    Logger.Log(e);
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return null;
        }

        public async Task<string> Post(string url, object data)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string requestUri = url;
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    response.EnsureSuccessStatusCode();
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                catch (HttpRequestException e)
                {
                    Logger.Log(e);
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return null;
        }
    }
}
