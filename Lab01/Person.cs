using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Lab01
{
    /// <summary>
    /// Class holding person data
    /// </summary>
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public BitmapImage Picture { get; set; } = new BitmapImage(new Uri(@"Resources\pudzian.jpg", UriKind.Relative));
    }
}
