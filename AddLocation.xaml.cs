using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace MSBeverageRecordApp {
    public partial class AddLocation : Page {
        class PostResponse {
            public int Id { get; set; }
        }

        class PostLocation {
            public string locationName { get; set; }
        }

        public class Location {
            public int id { get; set; }
            public string locationName { get; set; }
        }

        public class RootObject {
            public List<Location> LocationItems { get; set; }
        }

        RootObject deserializeObject = new RootObject();

        public AddLocation() {
            InitializeComponent();
            LocationAPI();
            CreateLocationComboBox(deserializeObject);
        }

        private void LocationAPI() {
            using HttpClient client = new();

            client.BaseAddress = new Uri("http://localhost:4001/api/location");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode) {
                var dataobjects = response.Content.ReadAsStringAsync().Result;

                deserializeObject.LocationItems = JsonSerializer.Deserialize<List<Location>>(dataobjects);
            }
        }

        private void Location_Button_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(txtLocation.Text)) {
                MessageBox.Show("Please enter a value in the location.");
            } else {
                string input = txtLocation.Text.ToUpper();
                txtLocation.Text = "";

                bool exists = deserializeObject.LocationItems.Any(l => l.locationName == input);

                if (exists) {
                    MessageBox.Show("This location already exists.");
                } else {
                    var postData = new PostLocation {
                        locationName = input
                    };

                    var client = new HttpClient();

                    client.BaseAddress = new Uri("http://localhost:4001/api/location/locationcreate/");

                    var json = System.Text.Json.JsonSerializer.Serialize(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(" ", content).Result;

                    if (response.IsSuccessStatusCode) {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var options = new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        };
                        MessageBox.Show("New Location Added");
                    }

                    this.NavigationService.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
                }
            }
        }


        private void CreateLocationComboBox(RootObject list) {
            bool contains = false;

            for (int index = 0; index < list.LocationItems.Count; index++) {
                for (int itemIndex = 0; itemIndex < cboLoc.Items.Count; itemIndex++)

                    if (cboLoc.Items[itemIndex].ToString() == list.LocationItems[index].locationName) {
                        contains = true;
                    }

                if (list.LocationItems[index].locationName == " " || list.LocationItems[index].locationName == "") {
                    index++;
                }

                if (contains == false) {
                    cboLoc.Items.Add(list.LocationItems[index].locationName);
                }
                contains = false;
            }
        }
    }
}
