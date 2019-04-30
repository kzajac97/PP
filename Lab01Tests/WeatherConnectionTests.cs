using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab01.Tests
{
    [TestClass()]
    public class WeatherConnectionTests
    {
        [TestMethod()]
        [Timeout(5000)]
        public async Task LoadDataAsyncTestAsync()
        {
            string responseXML = await WeatherConnection.LoadDataAsync("London");
            Assert.IsNotNull(responseXML);
        }

        [TestMethod()]
        [Timeout(5000)]
        public async Task LoadForecastAsyncTestAsync()
        {
            string responseXML = await WeatherConnection.LoadForecastAsync("London");
            Assert.IsNotNull(responseXML);
        }
    }
}
