using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab01
{
    /// <summary>
    /// Class responsible for Loading Data from APIs
    /// </summary>
    public class APIConnection
    {
        /// <summary>
        /// Loads data from given url
        /// </summary>
        /// <param name="apiUrl">
        /// API url
        /// </param>
        /// <returns></returns>
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
