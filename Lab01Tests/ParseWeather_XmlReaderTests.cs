using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab01.Tests
{
    [TestClass()]
    public class ParseWeather_XmlReaderTests
    {
        [TestMethod()]
        [Timeout(5000)]
        public async Task ParseTestAsync()
        {
            string responseXML = await WeatherConnection.LoadDataAsync("London");
            WeatherDataEntry result;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
            {
                result = ParseWeather_XmlReader.Parse(stream);
            }           

            Assert.IsNotNull(result.City);
        }
    }
}
