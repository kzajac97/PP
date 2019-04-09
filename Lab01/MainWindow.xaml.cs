using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace Lab01
{
    public partial class MainWindow : Window
    {
        BackgroundWorker worker = new BackgroundWorker();
        /// <summary>
        /// Asynchronous function to increment result
        /// <param name="number"/>
        /// Decides how long will this method work in background *100ms
        /// </param>
        /// </summary>
        async Task<int> GetNumberAsync(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException("number", number, "The number must be greater or equal zero");
            int result = 0;
            while (result < number)
            {
                result++;
                await Task.Delay(100);
            }
            return number;
        }
        /// <summary>
        /// Updates progress block informing about calculations progress
        /// </summary>
        protected void UpdateProgressBlock(string text, TextBlock textBlock)
        {
            int a = 2;
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = text + a.ToString();
                });
            }
            catch { } 
        }


        class WaitingAnimation
        {
            private int maxNumberOfDots;
            private int currentDots;
            private MainWindow sender;

            /// <summary>
            /// Constructor for WaitingAnimation
            /// <param name="maxNumberOfDots">
            /// Number of dots displayed
            /// </param>
            /// <param name="sender">
            /// Interaction logic from MainWindow
            /// </param>
            /// </summary>
            public WaitingAnimation(int maxNumberOfDots, MainWindow sender)
            {
                this.maxNumberOfDots = maxNumberOfDots;
                this.sender = sender;
                currentDots = 0;
            }

            public void CheckStatus(Object stateInfo)
            {
                sender.UpdateProgressBlock(
                    "Processing" +
                    new Func<string>(() =>
                    {
                        StringBuilder strBuilder = new StringBuilder(string.Empty);
                        for (int i = 0; i < currentDots; i++)
                            strBuilder.Append(".");
                        return strBuilder.ToString();
                    })(), sender.progressTextBlock
                );
                if (currentDots == maxNumberOfDots)
                    currentDots = 0;
                else
                    currentDots++;
            }
        }
        /// <summary>
        /// path to person image file
        /// </summary>
        string imagePath;
        
        /// <summary>
        /// observable collection displayed in MainWindow
        /// </summary>
        ObservableCollection<Person> people = new ObservableCollection<Person>
        {
            new Person { Name = "P1", Age = 1 },
            new Person { Name = "P2", Age = 2 }
        };

        public ObservableCollection<Person> Items
        {
            get => people;
        }

        /// <summary>
        /// static HttpClient field used not to create
        /// new instance with every iteration
        /// </summary>
        public static HttpClient Client => client;
        private readonly static HttpClient client = new HttpClient();

        public System.Windows.Media.Color BackgroundColor = Properties.Settings.Default.BackgroundColor;
        
        public void AddPerson(Person person)
        {
            Application.Current.Dispatcher.Invoke(() => { Items.Add(person); });
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
                people.Add(new Person { Age = age, Name = nameTextBox.Text, Picture = (BitmapImage)photoPreview.Source });              
            }

            else
            {
                MessageBox.Show("Age must be a number");
            }
        }

        Entity_Data_Modells.WeatherEntities db = new Entity_Data_Modells.WeatherEntities();
        System.Windows.Data.CollectionViewSource weatherEntryViewSource;
        System.Windows.Data.CollectionViewSource weatherEntitiesViewSource;
        private bool ColorSettingHandle;

        /// <summary>
        /// Adds new person from web every 5 seconds
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;

            weatherEntryViewSource = (CollectionViewSource)FindResource("weatherEntryViewSource");
            weatherEntitiesViewSource = (CollectionViewSource)FindResource("weatherEntitiesViewSource");

            IdLabel.Background = new SolidColorBrush(BackgroundColor);
            CityLabel.Background = new SolidColorBrush(BackgroundColor);
            TemperatureLabel.Background = new SolidColorBrush(BackgroundColor);
            PeopleLabel.Background = new SolidColorBrush(BackgroundColor);
            PhotosLabel.Background = new SolidColorBrush(BackgroundColor);
            WeatherLabel.Background = new SolidColorBrush(BackgroundColor);

            Debug.WriteLine("Bckg val");
            Debug.WriteLine(BackgroundColor);

            RunPeriodically(OnTick, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)).ContinueWith(task => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db.WeatherEntries.Local.Concat(db.WeatherEntries.ToList());
            weatherEntryViewSource.Source = db.WeatherEntries.Local;
            weatherEntitiesViewSource.Source = db.WeatherEntries.Local;
        }

        /// <summary>
        /// Adds New person to observable collection using HttpClient
        /// Uses Regex to get Webpage title and first encountered int
        /// </summary>
        private async void AddNewPersonFromWeb()
        {
            try
            {
                string result = await Client.GetStringAsync("https://pl.wikipedia.org/wiki/Specjalna:Losowa_strona");

                string name = Regex.Match(result, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                name = name.Replace(" � Wikipedia, wolna encyklopedia", "");
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
            if (dueTime > TimeSpan.Zero)
            {
                await Task.Delay(dueTime);
            }

            foreach (var item in Items)
            {
                Debug.WriteLine(item.Name, item.Age);
            }

            while (true)
            {
                OnTick?.Invoke();

                if (interval > TimeSpan.Zero)
                {
                    await Task.Delay(interval);
                }
            }
        }

        /// <summary>
        /// Adds new Person from web when called 
        /// </summary>
        private void OnTick() => AddNewPersonFromWeb();

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(idTextBox.Text, out int id) && double.TryParse(temperatureTextBox.Text, out double temperature))
            {
                var newEntry = new Entity_Data_Modells.WeatherEntry()
                {
                    Id = id,
                    City = cityTextBox.Text,
                    Temperature = temperature
                };
                db.WeatherEntries.Local.Add(newEntry);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.WeatherEntries.Local.Remove(newEntry);
                    MessageBox.Show("Error, id is not unique!");
                }
            }

            else
            {
                MessageBox.Show("Incorrect Data");
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(nameTextBox.Text, out int finalNumber))
                {
                    var getResultTask = GetNumberAsync(finalNumber);
                    var waitingAnimationTask =
                        new System.Threading.Timer(
                            new WaitingAnimation(10, this).CheckStatus,
                            null,
                            TimeSpan.FromMilliseconds(0),
                            TimeSpan.FromMilliseconds(500)
                        );
                    var waitingAnimationTask2 = new System.Timers.Timer(100);
                    waitingAnimationTask2.Elapsed +=
                        (innerSender, innerE) =>
                        {
                            this.UpdateProgressBlock(
                                innerE.SignalTime.ToLongTimeString(),
                                this.progressTextBlock2);
                        };
                    waitingAnimationTask2.Disposed +=
                        (innerSender, innerE) =>
                        {
                            this.progressTextBlock2.Text = "Koniec świata";
                        };
                    waitingAnimationTask2.Start();
                    int result = await getResultTask;
                    waitingAnimationTask.Dispose();
                    waitingAnimationTask2.Dispose();
                    this.progressTextBlock.Text = "Obtained result: " + result;
                }
                else
                {
                    MessageBox.Show("Error! Value must be an integer!");
                }
            }
            catch (Exception ex)
            {
                this.progressTextBlock.Text = "Error! " + ex.Message;
            }

        }

        private async void LoadWeatherData(object sender, RoutedEventArgs e)
        {
            string responseXML = await WeatherConnection.LoadDataAsync("London");
            WeatherDataEntry result;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
            {
                result = ParseWeather_XmlReader.Parse(stream);
                Items.Add(new Person()
                {
                    Name = "StreamParser: " + result.City,
                    Age = (int)Math.Round(result.Temperature)
                });
            }

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
            {
                result = ParseWeather_LINQ.Parse(stream);
                Items.Add(new Person()
                {
                    Name = "Linq: " + result.City,
                    Age = (int)Math.Round(result.Temperature)
                });
            }

            if (worker.IsBusy != true)
                worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            weatherDataProgressBar.Value = e.ProgressPercentage;
            weatherDataTextBlock.Text = e.UserState as string;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<string> cities = new List<string> {
                "London", "Warsaw", "Paris", "London", "Warsaw" };
            for (int i = 1; i <= cities.Count; i++)
            {
                string city = cities[i - 1];

                if (worker.CancellationPending == true)
                {
                    worker.ReportProgress(0, "Cancelled");
                    e.Cancel = true;
                    return;
                }
                else
                {
                    worker.ReportProgress(
                        (int)Math.Round((float)i * 100.0 / (float)cities.Count),
                        "Loading " + city + "...");
                    string responseXML = WeatherConnection.LoadDataAsync(city).Result;
                    WeatherDataEntry result;

                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
                    {
                        result = ParseWeather_XmlReader.Parse(stream);
                        AddPerson(
                            new Person()
                            {
                                Name = "StreamParser: " + result.City,
                                Age = (int)Math.Round(result.Temperature)
                            });
                    }
                    Thread.Sleep(2000);
                }
            }
            worker.ReportProgress(100, "Done");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (worker.WorkerSupportsCancellation == true)
            {
                weatherDataTextBlock.Text = "Cancelling...";
                worker.CancelAsync();
            }
        }

        private async void AddCatFact_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "https://cat-fact.herokuapp.com/facts/random";
            string response = await APIConnection.LoadDataAsync(apiUrl);
            try
            {
                Person result = JSONParser.ParseJSON(response, apiUrl);
                Items.Add(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ///<summary>
        ///Adds current bitcon value
        ///</summary>
        private async void AddBitcoinValue_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "https://api.coinranking.com/v1/public/coins?base=PLN&timePeriod=7d";
            string response = await APIConnection.LoadDataAsync(apiUrl);
            try
            {
                Person result = JSONParser.ParseJSON(response, apiUrl);
                Items.Add(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Adds current bitcon value
        /// </summary>
        private async void AddRandomFoxPhoto_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "https://randomfox.ca/floof/";
            string response = await APIConnection.LoadDataAsync(apiUrl);
            try
            {
                Person result = JSONParser.ParseJSON(response, apiUrl);
                Items.Add(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ColorSettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            ColorSettingHandle = !cmb.IsDropDownOpen;
            HandleColorSettingComboBox();
        }

        private void ColorSettingComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (ColorSettingHandle) HandleColorSettingComboBox();
            ColorSettingHandle = true;
        }

        private void HandleColorSettingComboBox()
        {
            switch (ColorSettingComboBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "RED":
                    Properties.Settings.Default.BackgroundColor = System.Windows.Media.Color.FromRgb(255, 0, 0);
                    Properties.Settings.Default.Save();                  
                    break;
                case "BLUE":
                    BackgroundColor = System.Windows.Media.Color.FromRgb(0, 0, 255);
                    break;
                case "GREEN":
                    BackgroundColor = System.Windows.Media.Color.FromRgb(0, 255, 0);
                    break;
                case "YELLOW":

                    break;
                default:
                    BackgroundColor = System.Windows.Media.Color.FromRgb(249,215,28);
                    break;
            }
        }
    }
}
