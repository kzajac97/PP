using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Lab01.Tests
{
    [TestClass()]
    public class APIConnectionTests
    {
        [TestMethod()]
        [Timeout(5000)]
        public async Task LoadDataAsyncTestAsync()
        {
            string apiUrl = "https://catfact.ninja/fact?max_length=140";
            string response = await APIConnection.LoadDataAsync(apiUrl);
            if (!response.Any())
            {
                Assert.Fail();
            }
        }    
    }
}
