using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using static Lab01.Person;
using static Lab01.APIConnection;
using System.Net;

namespace Lab01.Tests
{
    [TestClass()]
    public class JSONParserTests
    {
        [TestMethod()]
        [Timeout(5000)]
        public async Task ParseJSONTestAsync()
        {
            JSONParser parser = new JSONParser();
            string apiUrl = "https://catfact.ninja/fact?max_length=140";
            string response = await APIConnection.LoadDataAsync(apiUrl);

            try
            {
                Person result = JSONParser.ParseJSON(response, apiUrl);
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        [Timeout(5000)]
        public async Task ParseJSONForPlotTestAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string apiUrl = "http://api.coinranking.com/v1/public/coins?base=PLN&timePeriod=7d";
            string response = await APIConnection.LoadDataAsync(apiUrl);
            List<DataPoint> values = JSONParser.ParseJSONForPlot(response);
            if (!values.Any())
            {
                Assert.Fail();
            }
        }
    }
}
