using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using OxyPlot;
using System.Windows.Documents;
using System.Threading.Tasks;

namespace Lab01
{
    public class JSONParser
    {
        public string text;
        public int number;

        public static List<string> apiList = new List<string> {
                "https://catfact.ninja/fact?max_length=140", "http://api.coinranking.com/v1/public/coins?base=PLN&timePeriod=7d", "https://randomfox.ca/floof/" };

        public static List<DataPoint> ParseJSONForPlot(string result)
        {
            JToken token = JObject.Parse(result);
            List < DataPoint > prices = new List<DataPoint> { };
            double x, y;
            string myToken;
            for (int i = 0; i < 10; i++)
            {
                myToken = "data.coins[0].history[" + i + "]";
                x = i;
                y = (double)token.SelectToken(myToken);
                prices.Add(new DataPoint(x, y));
            }

            return prices;

        }


        public static Person ParseJSON(string result, string apiUrl)
        {

            JToken token = JObject.Parse(result);
            if (apiUrl == apiList[0])
            {
                string text = (string)token.SelectToken("fact");
                return new Person() { Name = text, Picture = new BitmapImage(new Uri(@"Resources\cat.jpg", UriKind.Relative)) };
            }

            else if (apiUrl == apiList[1])
            {
                string text = (string)token.SelectToken("data.coins[0].symbol");
                string number = (string)token.SelectToken("data.coins[0].price");

                if (double.TryParse(number, System.Globalization.NumberStyles.Any ,System.Globalization.CultureInfo.InvariantCulture, out double age))
                {
                    return new Person()
                    {
                        Name = text,
                        Age = (int)age
                    };
                }
                else
                {
                    throw new Exception("JSON Parser Failed");
                }
            }

            else if (apiUrl == apiList[2])
            {
                string image = (string)token.SelectToken("image");
                BitmapImage img = new BitmapImage();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        byte[] pic = client.DownloadData(image);


                        MemoryStream strmImg = new MemoryStream(pic);
                        BitmapImage myBitmapImage = new BitmapImage();
                        myBitmapImage.BeginInit();
                        myBitmapImage.StreamSource = strmImg;
                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();
                        img = myBitmapImage;
                    }
                    catch
                    {
                        throw new WebException("JSON Parser Failed");
                    }
                }
                return new Person()
                {
                    Picture = img
                };
            }

            else return new Person();
        }      
    }
}
