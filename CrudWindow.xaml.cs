using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MSBeverageRecordApp {

    public partial class CrudWindow : Page {




        string file = "";
        string[] rows = new string[1];
        int colCount = 0;
        string c = "";
        double equipmentCost = 0.0;
        PostRecordsData post = new PostRecordsData();
        Records rep = new Records();
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
            public int is_deleted { get; set; }
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
            public int is_deleted { get; set; }
            public bool isHidden { get; set; }
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


        public CrudWindow() {
            InitializeComponent();
            RecordsAPI();
            CategoryAPI();
            LocationAPI();
            ManufacturerAPI();
            comboboxSearch(deserializeObject);
            CreateCategoryComboBox(deserializeObject);
            CreateLocationComboBox(deserializeObject);
            CreateManufacturerComboBox(deserializeObject);
            MSBeverageRecordGrid.ItemsSource = deserializeObject.Items;
        }



        private void UpdateDataBase(RootObject list) {

            post = new PostRecordsData {
                record_id = post.record_id,
                category = post.category,
                manufacturer = post.manufacturer,
                model = post.model,
                serial = post.serial,
                purchase_date = post.purchase_date,
                cost = post.cost,
                location = post.location,
                sub_location = post.sub_location,
                is_deleted = post.is_deleted,
            };




            for (int itemIndex = 0; itemIndex < list.CategoryItems.Count; itemIndex++) {
                if (cboCatName.SelectedValue == list.CategoryItems[itemIndex].categoryName) {
                    post.category = list.CategoryItems[itemIndex].id;
                }
            }

            for (int itemIndex = 0; itemIndex < list.LocationItems.Count; itemIndex++) {
                if (cboLocation.SelectedValue == list.LocationItems[itemIndex].locationName) {
                    post.location = list.LocationItems[itemIndex].ID;
                }
            }

            for (int itemIndex = 0; itemIndex < list.ManufacturerItems.Count; itemIndex++) {
                if (cboManufacturer.SelectedValue == list.ManufacturerItems[itemIndex].companyName) {
                    post.manufacturer = list.ManufacturerItems[itemIndex].id;
                }
            }

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:4001/api/records/modifyid");
            var json = JsonSerializer.Serialize(post);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync("", content).Result;
            if (response.IsSuccessStatusCode) {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var postResponse = JsonSerializer.Deserialize<Records>(responseContent);
            } else {
                System.Windows.MessageBox.Show("Error " + response.StatusCode);
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            CreateCategoryComboBox(deserializeObject);
            CreateLocationComboBox(deserializeObject);
            CreateManufacturerComboBox(deserializeObject);

            Records Reports = new Records();
            var row = sender as DataGridRow;
            rep = row.DataContext as Records;
            Reports = rep;

            for (int i = 0; i < cboCatName.Items.Count; i++) {
                Debug.WriteLine(cboCatName.Items[i]);                
            }

            EditHeader.Content = $"Edit Record #{rep.record_id}";
            Debug.WriteLine($"Setting category: {rep.categoryName}");  // Debug log
            cboCatName.Text = $"{rep.categoryName}";
            cboManufacturer.Text = $"{rep.companyName}";
            txbModel.Text = $"{rep.model}";
            txbSerial.Text = $"{rep.serial}";
            txbPurchaseDate.Text = $"{DateTime.Today.ToString("d")}";
            txbCost.Text = $"{rep.cost}";
            cboLocation.Text = $"{rep.locationName}";
            txbSubLocation.Text = $"{rep.sub_location}";


            spLabels.Visibility = Visibility.Visible;
            spText.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            MSBeverageRecordGrid.Visibility = Visibility.Collapsed;
            consoleOutput.Visibility = Visibility.Collapsed;
            fileMenu.Visibility = Visibility.Collapsed;
            Filterby.Visibility = Visibility.Collapsed;
            txtSearchPlaceholder.Visibility = Visibility.Collapsed;
            FilterTextBox.Visibility = Visibility.Collapsed;

        }


        private void btnSaveChange_Click(object sender, RoutedEventArgs e) {

            Records Reports = rep;
            for (int i = 0; i < deserializeObject.Items.Count; i++) {

                if (deserializeObject.Items[i].record_id == Reports.record_id) {

                    post.record_id = Reports.record_id;
                    post.model = txbModel.Text;
                    post.serial = txbSerial.Text;
                    post.purchase_date = DateTime.Parse(txbPurchaseDate.Text);
                    post.cost = decimal.Parse(txbCost.Text);
                    post.sub_location = txbSubLocation.Text;

                    deserializeObject.Items[i].categoryName = cboCatName.Text;
                    deserializeObject.Items[i].companyName = cboManufacturer.Text;
                    deserializeObject.Items[i].model = txbModel.Text;
                    deserializeObject.Items[i].serial = txbSerial.Text;
                    deserializeObject.Items[i].purchase_date = DateTime.Parse(txbPurchaseDate.Text);
                    deserializeObject.Items[i].cost = decimal.Parse(txbCost.Text);
                    deserializeObject.Items[i].locationName = cboLocation.Text;
                    deserializeObject.Items[i].sub_location = txbSubLocation.Text;
                }

                EditHeader.Content = "";
                MSBeverageRecordGrid.ItemsSource = null;
                MSBeverageRecordGrid.ItemsSource = deserializeObject.Items;

            }
            UpdateDataBase(deserializeObject);

            MSBeverageRecordGrid.Visibility = Visibility.Visible;
            consoleOutput.Visibility = Visibility.Hidden;
            fileMenu.Visibility = Visibility.Visible;
            Filterby.Visibility = Visibility.Visible;
            txtSearchPlaceholder.Visibility = Visibility.Visible;
            FilterTextBox.Visibility = Visibility.Visible;
            Filterby.Visibility = Visibility.Visible;
            txtSearchPlaceholder.Visibility = Visibility.Visible;
            FilterTextBox.Visibility = Visibility.Visible;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;

            spLabels.Visibility = Visibility.Hidden;
            spText.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {

            Records Reports = rep;

            MessageBoxResult dialogResult = MessageBox.Show($"delete record ID: {rep.record_id} \nare you sure?", $"record: {rep.record_id}", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes) {

                Reports.isHidden = true;
                for (int i = 0; i < deserializeObject.Items.Count; i++) {

                    if (deserializeObject.Items[i].record_id == Reports.record_id) {
                        deserializeObject.Items[i].is_deleted = 1;

                        post.record_id = Reports.record_id;
                        post.model = txbModel.Text;
                        post.serial = txbSerial.Text;
                        post.purchase_date = DateTime.Parse(txbPurchaseDate.Text);
                        post.cost = decimal.Parse(txbCost.Text);
                        post.sub_location = txbSubLocation.Text;
                        post.is_deleted = 1;
                    }

                }

                UpdateDataBase(deserializeObject);

                MSBeverageRecordGrid.Visibility = Visibility.Visible;
                fileMenu.Visibility = Visibility.Visible;
                Filter.Visibility = Visibility.Visible;

                btnSave.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;

                Filterby.Visibility = Visibility.Visible;
                txtSearchPlaceholder.Visibility = Visibility.Visible;
                FilterTextBox.Visibility = Visibility.Visible;

                consoleOutput.Visibility = Visibility.Hidden;
                spLabels.Visibility = Visibility.Hidden;
                spText.Visibility = Visibility.Hidden;
                EditHeader.Content = "";
            } else if (dialogResult == MessageBoxResult.No) {

            }
            MSBeverageRecordGrid.Items.Refresh();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            MSBeverageRecordGrid.Visibility = Visibility.Visible;
            consoleOutput.Visibility = Visibility.Hidden;
            fileMenu.Visibility = Visibility.Visible;
            Filterby.Visibility = Visibility.Visible;
            txtSearchPlaceholder.Visibility = Visibility.Visible;
            FilterTextBox.Visibility = Visibility.Visible;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;

            spLabels.Visibility = Visibility.Hidden;
            spText.Visibility = Visibility.Hidden;

            EditHeader.Content = "";
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

                for (int i = 0; i < deserializeObject.Items.Count; i++) {
                    if (deserializeObject.Items[i].sub_location == null) {
                        deserializeObject.Items[i].sub_location = "";
                    }
                }
            }
        }

        private void CategoryAPI() {

            using HttpClient client = new();

            client.BaseAddress = new Uri("http://localhost:4001/api/category");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode) {

                var dataobjects = response.Content.ReadAsStringAsync().Result;

                deserializeObject.CategoryItems = JsonSerializer.Deserialize<List<Category>>(dataobjects);
                for (int i = 0; i < deserializeObject.Items.Count; i++) {
                    if (deserializeObject.Items[i].sub_location == null) {
                        deserializeObject.Items[i].sub_location = "";
                    }
                }
            }
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
                for (int i = 0; i < deserializeObject.Items.Count; i++) {
                    if (deserializeObject.Items[i].sub_location == null) {
                        deserializeObject.Items[i].sub_location = "";
                    }
                }
            }
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
                for (int i = 0; i < deserializeObject.Items.Count; i++) {
                    if (deserializeObject.Items[i].sub_location == null) {
                        deserializeObject.Items[i].sub_location = "";
                    }
                }
            }
        }

        private void CreateCategoryComboBox(RootObject list) {
            cboCatName.Items.Clear();  // Clear existing items
            bool contains = false;

            for (int index = 0; index < list.CategoryItems.Count; index++) {
                for (int itemIndex = 0; itemIndex < cboCatName.Items.Count; itemIndex++) {
                    if (cboCatName.Items[itemIndex].ToString() == list.CategoryItems[index].categoryName) {
                        contains = true;
                        break;  // Break the loop if the item is found
                    }
                }

                if (!contains) {

                    cboCatName.Items.Add(list.CategoryItems[index].categoryName);
                }
                contains = false;  // Reset the contains flag
            }
        }

        private void CreateLocationComboBox(RootObject list) {
            cboLocation.Items.Clear();  // Clear existing items
            bool contains = false;

            for (int index = 0; index < list.LocationItems.Count; index++) {
                for (int itemIndex = 0; itemIndex < cboLocation.Items.Count; itemIndex++) {
                    if (cboLocation.Items[itemIndex].ToString() == list.LocationItems[index].locationName) {
                        contains = true;
                        break;  // Break the loop if the item is found
                    }
                }

                if (!contains) {
                    cboLocation.Items.Add(list.LocationItems[index].locationName);
                }
                contains = false;  // Reset the contains flag
            }
        }

        private void CreateManufacturerComboBox(RootObject list) {
            cboManufacturer.Items.Clear();  // Clear existing items
            bool contains = false;

            for (int index = 0; index < list.ManufacturerItems.Count; index++) {
                for (int itemIndex = 0; itemIndex < cboManufacturer.Items.Count; itemIndex++) {
                    if (cboManufacturer.Items[itemIndex].ToString() == list.ManufacturerItems[index].companyName) {
                        contains = true;
                        break;  // Break the loop if the item is found
                    }
                }

                if (!contains) {
                    
                    cboManufacturer.Items.Add(list.ManufacturerItems[index].companyName);
                }
                contains = false;  // Reset the contains flag
            }
        }





        #region sub combobox search


        private void comboboxSearch(RootObject deserializedObjectList) {
            Filterby.ItemsSource = deserializedObjectList.Items;

            Filterby.ItemsSource = new string[] { "All", "RecordID", "Category", "Manufacturer", "Model", "SerialNumber", "Location" };
        }


        public Predicate<object> GetFilter() {

            switch (Filterby.SelectedItem as string) {

                case null:
                    return NoFilter;

                case "RecordID":
                    return RecordIDFilter;

                case "Category":
                    return CategoryFilter;

                case "Manufacturer":
                    return ManufacturerFilter;

                case "Model":
                    return ModelFilter;

                case "SerialNumber":
                    return SerialNumberFilter;

                case "Location":
                    return LocationFilter;

                case "All":
                    return NoFilter;

            }

            return RecordIDFilter;
        }

        //returns all id's containing the search? 
        //check other comparisons
        private bool NoFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.record_id.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.categoryName.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.model.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.companyName.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.serial.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.locationName.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.cost.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.sub_location.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase) || Filterobj.purchase_date.ToString().Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool RecordIDFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.record_id.ToString().Equals(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool CategoryFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.categoryName.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool ManufacturerFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.companyName.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool ModelFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.model.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool SerialNumberFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.serial.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private bool LocationFilter(object obj) {
            var Filterobj = obj as Records;

            return Filterobj.locationName.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);

        }


        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e) {

            if (FilterTextBox.Text == "") {
                MSBeverageRecordGrid.Items.Filter = null;
                txtSearchPlaceholder.Visibility = System.Windows.Visibility.Visible;
            } else {
                MSBeverageRecordGrid.Items.Filter = GetFilter();
                txtSearchPlaceholder.Visibility = System.Windows.Visibility.Hidden;

            }
        }


        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            MSBeverageRecordGrid.Items.Filter = GetFilter();

            switch (Filterby.SelectedItem as string) {
                case "Category":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Category";
                    foreach (var categoryItem in deserializeObject.CategoryItems) {
                        cboSearch.Items.Add(categoryItem.categoryName);
                    }
                    break;
                case "Manufacturer":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Manufacturer";
                    foreach (var manufacturerItem in deserializeObject.ManufacturerItems) {
                        cboSearch.Items.Add(manufacturerItem.companyName);
                    }
                    break;
                case "Location":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Location";
                    foreach (var locationItem in deserializeObject.LocationItems) {
                        cboSearch.Items.Add(locationItem.locationName);
                    }
                    break;
                default:
                    cboSearch.Visibility = Visibility.Hidden;
                    clrSearch.Visibility = Visibility.Hidden;
                    break;
            }


        }

        private void cboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (cboSearch.SelectedItem != null) {
                string selected = cboSearch.SelectedItem.ToString();
                string selectedFilter = Filterby.SelectedItem.ToString();
                switch (selectedFilter) {
                    case "Category":
                        MSBeverageRecordGrid.Items.Filter = item =>
                            ((Records)item).categoryName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "Manufacturer":
                        MSBeverageRecordGrid.Items.Filter = item =>
                            ((Records)item).companyName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "Location":
                        MSBeverageRecordGrid.Items.Filter = item =>
                            ((Records)item).locationName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                }

            }
        }

        private void clrSearch_Click(object sender, RoutedEventArgs e) {
            try {
                MSBeverageRecordGrid.Items.Filter = null;

                cboSearch.SelectedItem = null;
                Filterby.SelectedItem = "";
                MSBeverageRecordGrid.ItemsSource = deserializeObject.Items;
                MSBeverageRecordGrid.Items.Refresh();
            } catch (Exception ex) {
                Console.WriteLine("Error in clrSearch_Click: " + ex.Message);
            }
            Filterby.Text = "Search By";
            clrSearch.Visibility = Visibility.Hidden;
            cboSearch.Visibility = Visibility.Hidden;
        }


        #endregion sub combobox search


    }
}