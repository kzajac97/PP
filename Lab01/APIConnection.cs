using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab01
{
    public class APIConnection
    {
        public static async Task<string> LoadDataAsync(string apiUrl)
        {
            Task<string> result;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(apiUrl))
            using (HttpContent content = response.Content)
            {
                try
                {
                    result = content.ReadAsStringAsync();
                }

                catch
                {
                    throw new HttpRequestException("Api Conncection Failed");
                }
            }
            return await result;
        }
    }
}
