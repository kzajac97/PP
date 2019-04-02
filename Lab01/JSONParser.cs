using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Lab01
{
    public class JSONParser
    {
        public string text;
        public int number;

        public static WeatherDataEntry ParseJSON(string result, string apiUrl)
        {
            
            JToken token = JObject.Parse(result);
            if (apiUrl == "https://cat-fact.herokuapp.com/facts/random")
            {
                string text = (string)token.SelectToken("text");
                return new WeatherDataEntry() { City = text };
            }

            else if (apiUrl == "https://api.coinranking.com/v1/public/coins?base=PLN&timePeriod=7d")
            {
                string text = (string)token.SelectToken("data.coins[0].symbol");
                string number = (string)token.SelectToken("data.coins[0].price");
                return new WeatherDataEntry()
                {
                    City = text,
                    Temperature = float.Parse(number, System.Globalization.CultureInfo.InvariantCulture.NumberFormat)
                };
            }

            else return new WeatherDataEntry();
        }
    }
}
