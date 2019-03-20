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
using System.Windows.Interop;
using System.Drawing;

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

        /// <summary>
        /// observable collection displayed in MainWindow
        /// </summary>
        public ObservableCollection<Person> Items => people;
        static ObservableCollection<Person> people = new ObservableCollection<Person>
        {
            new Person { Name = "P1", Age = 1 },
            new Person { Name = "P2", Age = 2 }
        };

        /// <summary>
        /// static HttpClient field used not to create
        /// new instance with every iteration
        /// </summary>
        public static HttpClient Client => client;
        private readonly static HttpClient client = new HttpClient();

        /// <summary>
        /// Adds new person from web every 5 seconds
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            RunPeriodically(OnTick, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)).ContinueWith(task => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Adds New person to observable collection using HttpClient
        /// Uses Regex to get Webpage title and first encountered int
        /// </summary>
        private async void AddNewPersonFromWeb()
        {
            try
            {        
                string result = await Client.GetStringAsync("https://en.wikipedia.org/wiki/Main_Page");

                string name = Regex.Match(result, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                string ageString = Regex.Match(result, "[0-9]+").Value;
                
                if (int.TryParse(ageString, out int age))
                {
                    people.Add(new Person { Age = age, Name = name });
                }

                else
                {
                    MessageBox.Show("Age must be number");
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

        /// <summary>
        /// Event for AddNewPersonButton
        /// Adds new person with user's input data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Adds photo to person instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Title = "Select a photo" };

            if (fileDialog.ShowDialog() == true)
            {
                photoPreview.Source = new BitmapImage(new Uri(fileDialog.FileName));
                imagePath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Event for AddNewPersonFromWeb
        /// Adds new person from web manually
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewPersonFromWeb_Click(object sender, RoutedEventArgs e) => AddNewPersonFromWeb();

        /// <summary>
        /// Task running periodically calling OnTick event function 
        /// </summary>
        /// <param name="OnTick">
        /// Delegate to event happening OnTicks
        /// </param>
        /// <param name="dueTime">
        /// TimeSpan parameter
        /// </param>
        /// <param name="interval">
        /// Time span paramter setting length of inverval between OnTick calls
        /// </param>
        /// <returns>
        /// Returns Task so it can run asynchronously in MainWindow()
        /// </returns>
        private async Task RunPeriodically(Action OnTick, TimeSpan dueTime, TimeSpan interval)
        {
            if(dueTime > TimeSpan.Zero)
            {
                await Task.Delay(dueTime);
            }

            while(true)
            {
                OnTick?.Invoke();

                if(interval > TimeSpan.Zero)
                {
                    await Task.Delay(interval);
                }
            }
        }

        /// <summary>
        /// Adds new Person from web when called 
        /// </summary>
        private void OnTick() => AddNewPersonFromWeb();
    }
}
