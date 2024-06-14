using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace MSBeverageRecordApp {
    public partial class AddManufacturer : Page {
        class PostResponse {
            public int Id { get; set; }
        }

        class PostManufacturer {
            public string companyName { get; set; }
        }

        public class Manufacturer {
            public int id { get; set; }
            public string companyName { get; set; }
        }

        public class RootObject {
            public List<Manufacturer> ManufacturerItems { get; set; }
        }

        RootObject deserializeObject = new RootObject();

        public AddManufacturer() {
            InitializeComponent();
            ManufacturerAPI();
            CreateManufacturerComboBox(deserializeObject);

        }

        private void ManufacturerAPI() {
            using HttpClient client = new();

            client.BaseAddress = new Uri("http://localhost:4001/api/manufacturer");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode) {
                var dataobjects = response.Content.ReadAsStringAsync().Result;

                deserializeObject.ManufacturerItems = JsonSerializer.Deserialize<List<Manufacturer>>(dataobjects);
            }

        }

        private void Manufacturer_Button_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(txtManufacturer.Text)) {
                MessageBox.Show("Please enter a value in the manufacturer.");
            } else {
                string input = txtManufacturer.Text.ToUpper();
                txtManufacturer.Text = "";

                bool exists = deserializeObject.ManufacturerItems.Any(m => m.companyName == input);

                if (exists) {
                    MessageBox.Show("This manufacturer already exists.");
                } else {
                    var postData = new PostManufacturer {
                        companyName = input
                    };

                    var client = new HttpClient();

                    client.BaseAddress = new Uri("http://localhost:4001/api/manufacturer/manufacturercreate/");

                    var json = System.Text.Json.JsonSerializer.Serialize(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(" ", content).Result;

                    if (response.IsSuccessStatusCode) {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var options = new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        };
                        MessageBox.Show("New manufacturer Added");
                    }

                    this.NavigationService.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
                }
            }
        }


        private void CreateManufacturerComboBox(RootObject list) {
            bool contains = false;

            for (int index = 0; index < list.ManufacturerItems.Count; index++) {
                for (int itemIndex = 0; itemIndex < cboMan.Items.Count; itemIndex++)

                    if (cboMan.Items[itemIndex].ToString() == list.ManufacturerItems[index].companyName) {
                        contains = true;
                    }

                if (list.ManufacturerItems[index].companyName == " " || list.ManufacturerItems[index].companyName == "") {
                    index++;
                }

                if (contains == false) {
                    cboMan.Items.Add(list.ManufacturerItems[index].companyName);
                }
                contains = false;
            }

        }
    }
}