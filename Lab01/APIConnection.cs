using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace Lab01
{ 
    class APIConnection
    {
        public static async Task<string> LoadDataAsync(string apiUrl)
        {
           
            Task<string> result;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(apiUrl))
            using (HttpContent content = response.Content)
            {
                result = content.ReadAsStringAsync();
            }

            
            return await result;
        }
    }
}