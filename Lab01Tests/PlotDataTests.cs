using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Lab01.Tests
{
    [TestClass()]
    public class PlotDataTests
    {
        [TestMethod()]
        [Timeout(100)]
        public void addNewPointTest()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            DataPoint point = new DataPoint(5, 5);
            dataPoints.Add(point);

            Assert.AreEqual(point.X,dataPoints.Last().X);
            Assert.AreEqual(point.Y, dataPoints.Last().Y);
        }
    }
}
