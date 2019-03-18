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

namespace Lab01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string imagePath;
        ObservableCollection<Person> people = new ObservableCollection<Person>
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

        private async void AddNewPersonFromWeb_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            BitmapImage image = new BitmapImage();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("http://www.contoso.com");
                    response.EnsureSuccessStatusCode();

                    string name = await response.Content.ReadAsStringAsync();

                    name = new string((from c in name where char.IsWhiteSpace(c) || char.IsLetter(c) select c).ToArray());

                    using (MemoryStream stream = new MemoryStream())
                    {
                        await stream.CopyToAsync(stream);
                        image.BeginInit();
                        image.StreamSource = stream;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                    }

                    people.Add(new Person { Age = random.Next(1, 100), Name = name, Picture = image });
                }
            }

            catch(Exception ex)
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
    }
}
