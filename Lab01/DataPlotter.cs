using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Lab01
{
    /// <summary>
    /// Class responsible for plotting data using OxyPlot library
    /// </summary>
    public class PlotData
    {
        List<DataPoint> dataPoints;

        public void addNewPoint(DataPoint point)
        {
            dataPoints.Add(point);
        }
    }
}
