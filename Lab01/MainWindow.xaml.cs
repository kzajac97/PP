using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Timers;

namespace Lab01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// path to person image file
        /// </summary>
        string imagePath;

        static ObservableCollection<Person> people = new ObservableCollection<Person>
        {
            new Person { Name = "P1", Age = 1 },
            new Person { Name = "P2", Age = 2 }
        };

        public ObservableCollection<Person> Items => people;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void AddNewPersonFromWeb()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string result = await client.GetStringAsync("https://en.wikipedia.org/wiki/Main_Page");
                    string name = Regex.Match(result, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                    string ageString = Regex.Match(result, "[0-9]+").Value;
                    Trace.WriteLine(ageString);
                    if (int.TryParse(ageString, out int age))
                    { people.Add(new Person { Age = age, Name = name }); }
                    else
                    { MessageBox.Show("Age must be number"); }
                }
            }

            catch (Exception ex)
            {
                if (ex is HttpRequestException)
                {
                    MessageBox.Show(ex.Message);
                }

                else
                {
                    MessageBox.Show("Unkown Exception caught");
                }
            }
        }

        private void AddNewPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ageTextBox.Text, out int age))
            {
                people.Add(new Person { Age = age, Name = nameTextBox.Text, Picture = (BitmapImage) photoPreview.Source});
            }

            else
            {
                MessageBox.Show("Age must be a number");
            }
        }

        private void AddNewPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Title = "Select a photo" };

            if (fileDialog.ShowDialog() == true)
            {
                photoPreview.Source = new BitmapImage(new Uri(fileDialog.FileName));
                imagePath = fileDialog.FileName;
            }
        }

        private void AddNewPersonFromWeb_Click(object sender, RoutedEventArgs e)
        {
            AddNewPersonFromWeb();
        }
    }
}
