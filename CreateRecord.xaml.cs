using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;


namespace MSBeverageRecordApp {

    public partial class CreateRecord : Page {
        RootObject deserializeObject = new RootObject();

        class PostRecordsData {
            public int record_id { get; set; }
            public int category { get; set; }
            public int manufacturer { get; set; }
            public string model { get; set; }
            public string serial { get; set; }
            public DateTime purchase_date { get; set; }
            public decimal cost { get; set; }
            public int location { get; set; }
            public string sub_location { get; set; }
        }

        public class Records {
            public int record_id { get; set; }
            public string categoryName { get; set; }
            public string companyName { get; set; }
            public string model { get; set; }
            public string serial { get; set; }
            public DateTime purchase_date { get; set; }
            public decimal cost { get; set; }
            public string locationName { get; set; }
            public string sub_location { get; set; }
        }

        public class Category {
            public int id { get; set; }
            public string categoryName { get; set; }
        }

        public class Location {
            public int ID { get; set; }
            public string locationName { get; set; }
        }

        public class Manufacturer {
            public int id { get; set; }
            public string companyName { get; set; }
        }

        public class RootObject {
            public int id { get; set; }
            public List<Records> Items { get; set; }
            public List<Category> CategoryItems { get; set; }
            public List<Location> LocationItems { get; set; }
            public List<Manufacturer> ManufacturerItems { get; set; }
        }

        class PostResponse {
            public int Id { get; set; }
        }

        public CreateRecord() {
            InitializeComponent();

            RecordsAPI();
            CategoryAPI();
            LocationAPI();
            ManufacturerAPI();

            RecordIDTextBox(deserializeObject);
            CreateCategoryComboBox(deserializeObject);
            CreateLocationComboBox(deserializeObject);
            CreateManufacturerComboBox(deserializeObject);
            PurchaseDate.Text = $"{DateTime.Today.ToString("d")}";
        }


        private void RecordsAPI() {

            using HttpClient client = new();

            client.BaseAddress = new Uri("http://localhost:4001/api/records/recordsreal");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode) {

                var dataobjects = response.Content.ReadAsStringAsync().Result;

                deserializeObject.Items = JsonSerializer.Deserialize<List<Records>>(dataobjects);

            }
        }


        #region Category Table Combobox

        private void CategoryAPI() {

            using HttpClient client = new();

            client.BaseAddress = new Uri("http://localhost:4001/api/category");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode) {

                var dataobjects = response.Content.ReadAsStringAsync().Result;

                deserializeObject.CategoryItems = JsonSerializer.Deserialize<List<Category>>(dataobjects);

            }
        }

        private void CreateCategoryComboBox(RootObject list) {
            for (int index = 0; index < list.CategoryItems.Count; index++) {
                if (string.IsNullOrWhiteSpace(list.CategoryItems[index].categoryName)) {
                    continue;
                }

                bool contains = false;

                for (int itemIndex = 0; itemIndex < cboCategory.Items.Count; itemIndex++) {
                    if (cboCategory.Items[itemIndex].ToString() == list.CategoryItems[index].categoryName) {
                        contains = true;
                        break;
                    }
                }

                if (!contains) {
                    cboCategory.Items.Add(list.CategoryItems[index].categoryName);
                }
            }
        }

        #endregion Category Table Combobox


        #region Location Table Combobox
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


        private void CreateLocationComboBox(RootObject list) {
            for (int index = 0; index < list.LocationItems.Count; index++) {
                if (string.IsNullOrWhiteSpace(list.LocationItems[index].locationName)) {
                    continue;
                }

                bool contains = false;

                for (int itemIndex = 0; itemIndex < cboLocation.Items.Count; itemIndex++) {
                    if (cboLocation.Items[itemIndex].ToString() == list.LocationItems[index].locationName) {
                        contains = true;
                        break;
                    }
                }

                if (!contains) {
                    cboLocation.Items.Add(list.LocationItems[index].locationName);
                }
            }
        }
        #endregion Location Table Combobox


        #region Manufacturer Table Combobox
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


        private void CreateManufacturerComboBox(RootObject list) {
            for (int index = 0; index < list.ManufacturerItems.Count; index++) {
                if (string.IsNullOrWhiteSpace(list.ManufacturerItems[index].companyName)) {
                    continue;
                }

                bool contains = false;

                for (int itemIndex = 0; itemIndex < cboManufacturer.Items.Count; itemIndex++) {
                    if (cboManufacturer.Items[itemIndex].ToString() == list.ManufacturerItems[index].companyName) {
                        contains = true;
                    }
                }

                if (!contains) {
                    cboManufacturer.Items.Add(list.ManufacturerItems[index].companyName);
                }
            }
        }

        #endregion Manufacturer Table Combobox


        #region RecordID
        private void RecordIDTextBox(RootObject list) {

            int currentRecordID = 0;

            for (int index = 0; index < list.Items.Count; index++) {

                int newCurrentRecordID = 0;

                newCurrentRecordID = list.Items[index].record_id;

                if (newCurrentRecordID > currentRecordID) {
                    currentRecordID = newCurrentRecordID;
                    recordNumber.Text = $"{currentRecordID + 1}";
                }
            }

        }

        private void recordNumber_TextChanged(object sender, TextChangedEventArgs e) {
            if (recordNumber.Text != "") {
                IDtxtSearchPlaceholder.Visibility = Visibility.Hidden;
            } else {
                IDtxtSearchPlaceholder.Visibility = Visibility.Hidden;
            }
        }
        #endregion RecordID


        private void txtModel_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtModel.Text != "") {
                modeltxtSearchPlaceholder.Visibility = Visibility.Hidden;
            } else {
                modeltxtSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }


        private void txtSerialNumber_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtSerialNumber.Text != "") {
                serialtxtSearchPlaceholder.Visibility = Visibility.Hidden;
            } else {
                serialtxtSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }


        private void txtCost_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtCost.Text != "") {
                costtxtSearchPlaceholder.Visibility = Visibility.Hidden;
            } else {
                costtxtSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }


        private void txtSubLocation_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtSubLocation.Text != "") {
                subLocationtxtSearchPlaceholder.Visibility = Visibility.Hidden;
            } else {
                subLocationtxtSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PostNewRecords(RootObject list) {
            var postData = new PostRecordsData {
                model = txtModel.Text.ToUpper(),
                serial = txtSerialNumber.Text.ToUpper(),
                purchase_date = PurchaseDate.SelectedDate.Value,
                cost = decimal.Parse(txtCost.Text),
                sub_location = txtSubLocation.Text.ToUpper()
            };

            for (int itemIndex = 0; itemIndex < list.CategoryItems.Count; itemIndex++) {
                if (cboCategory.SelectedValue == list.CategoryItems[itemIndex].categoryName) {
                    postData.category = list.CategoryItems[itemIndex].id;
                }
            }

            for (int itemIndex = 0; itemIndex < list.LocationItems.Count; itemIndex++) {
                if (cboLocation.SelectedValue == list.LocationItems[itemIndex].locationName) {
                    postData.location = list.LocationItems[itemIndex].ID;
                }
            }

            for (int itemIndex = 0; itemIndex < list.ManufacturerItems.Count; itemIndex++) {
                if (cboManufacturer.SelectedValue == list.ManufacturerItems[itemIndex].companyName) {
                    postData.manufacturer = list.ManufacturerItems[itemIndex].id;
                }
            }

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:4001/api/records/recordscreate");

            var json = System.Text.Json.JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(" ", content).Result;

            if (response.IsSuccessStatusCode) {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                };

                MessageBox.Show("New Record Created. Please add another record, or return to main menu.");

            }

        }


        private void btnSubmit_Click(object sender, System.Windows.RoutedEventArgs e) {

            if (string.IsNullOrEmpty(txtModel.Text) || string.IsNullOrEmpty(txtSerialNumber.Text) || string.IsNullOrEmpty(txtCost.Text) || string.IsNullOrEmpty(PurchaseDate.Text) || cboCategory.SelectedValue == null || cboLocation.SelectedValue == null || cboManufacturer.SelectedValue == null) {
                MessageBox.Show("Please ensure all fields are completed.");
            } else {
                PostNewRecords(deserializeObject);

                this.NavigationService.Refresh();

            }
        }

    }
}
