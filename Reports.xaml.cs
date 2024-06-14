using ChoETL;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ComboBox = System.Windows.Controls.ComboBox;

namespace MSBeverageRecordApp {


    //reset the cost to the total for all records after switch out and back into the all data tab

    public partial class Reports : Page {
        decimal totalBox = 0.0m;
        bool isSubLocationFiltering = false;

        RootObject deserializeObject = new RootObject();

        string filterName = "allData";
        public List<Records> allReports;

        public class urlResult {
            public string[] results { get; set; }
        }

        RootObject allObj = new RootObject();
        DataGrid allGrid = new DataGrid();

        RootObject catObj = new RootObject();
        DataGrid catGrid = new DataGrid();

        RootObject manuObj = new RootObject();
        DataGrid manuGrid = new DataGrid();

        RootObject locObj = new RootObject();
        DataGrid locGrid = new DataGrid();

        RootObject s_locObj = new RootObject();
        DataGrid s_locGrid = new DataGrid();

        RootObject totalObj = new RootObject();
        DataGrid totalGrid = new DataGrid();
        string title = "";
        private List<Records> filteredRecords;



        public Reports() {
            InitializeComponent();
            LoadData();
        }

        //fetch data from api for grid
        private async void LoadData() {
            try {
                using HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:4001/api/records/recordsreal");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(client.BaseAddress);
                if (response.IsSuccessStatusCode) {
                    var dataobjects = await response.Content.ReadAsStringAsync();
                    deserializeObject.Items = JsonSerializer.Deserialize<List<Records>>(dataobjects);
                    if (deserializeObject.Items != null) {
                        MSBeverageRecordApp.ItemsSource = deserializeObject.Items;
                        allObj = deserializeObject;
                        allGrid.ItemsSource = allObj.Items;
                        MSBeverageRecordApp2.ItemsSource = deserializeObject.Items;
                        MSBeverageRecordApp3.ItemsSource = deserializeObject.Items;
                        MSBeverageRecordApp4.ItemsSource = deserializeObject.Items;

                        if (deserializeObject.Items.Count > 0) {
                            var costReport = CreateCostReport(deserializeObject);
                            MSBeverageRecordApp5.ItemsSource = costReport;
                            totalGrid.ItemsSource = costReport;
                            totalObj = deserializeObject;
                            totalObj.CostItems = (List<TotalCostData>)totalGrid.ItemsSource;
                        }
                        CreateCategoryFilterItems(deserializeObject);
                        CreateManufacturerFilterItems(deserializeObject);
                        CreateLocationFilterItems(deserializeObject);
                        CostTotal(deserializeObject);

                    } else {
                        MessageBox.Show("Failed to deserialize items.");
                    }
                } else {
                    MessageBox.Show("Failed to retrieve data from API.");
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void CostTotal(RootObject d) {
            if (d != null) {
                foreach (var item in d.Items) {
                    totalBox += item.cost;
                }
                lblCost.Content = $"cost: {totalBox.ToString("C")}";
            } 
            
        }


        #region print calls

        private void muiPrint_Click(object sender, RoutedEventArgs e) {

            PrintDG p = new PrintDG();

            switch (filterName) {
                case "allData":

                    title = $"report    {DateTime.UtcNow.ToString("d")}";
                    p.printDG(allObj, MSBeverageRecordApp, title, filterName);
                    break;
                case "category":
                    title = $"category report    {DateTime.UtcNow.ToString("d")}";
                    p.printDG(catObj, MSBeverageRecordApp2, title, filterName);

                    break;
                case "manufacturer":
                    title = $"manufacturers    {DateTime.UtcNow.ToString("d")}";
                    p.printDG(manuObj, MSBeverageRecordApp3, title, filterName);
                    break;
                case "location":
                    if (FilterSubLocation.Text != "Select Sub-Location") {
                        title = $"sublocation report    {DateTime.UtcNow.ToString("d")}";
                        p.printDG(s_locObj, MSBeverageRecordApp4, title, filterName);
                    } else {
                        title = $"location report    {DateTime.UtcNow.ToString("d")}";
                        p.printDG(locObj, MSBeverageRecordApp4, title, filterName);
                    }
                    break;
                case "totalValue":
                    title = $"cost report    {DateTime.UtcNow.ToString("d")}";
                    p.printDG(totalObj, MSBeverageRecordApp5, title, filterName);
                    break;
            }
        }
        #endregion



        #region save calls

        private void muiSavePDF_Click(object sender, RoutedEventArgs e) {


            SaveReport s = new SaveReport();

            switch (filterName) {
                case "allData":

                    title = $"report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToPdf(allObj, MSBeverageRecordApp, title, filterName);
                    break;
                case "category":
                    title = $"category report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToPdf(catObj, MSBeverageRecordApp2, title, filterName);
                    break;
                case "manufacturer":
                    s.ExportToPdf(manuObj, MSBeverageRecordApp3, title, filterName);
                    title = $"manufacturers    {DateTime.UtcNow.ToString("d")}";
                    break;
                case "location":
                    s.ExportToPdf(locObj, MSBeverageRecordApp4, title, filterName);
                    title = $"location report    {DateTime.UtcNow.ToString("d")}";
                    break;
                case "totalValue":
                    s.ExportToPdf(totalObj, MSBeverageRecordApp5, title, filterName);
                    title = $"cost report    {DateTime.UtcNow.ToString("d")}";
                    break;
            }

        }

        private void muiSaveCSV_Click(object sender, RoutedEventArgs e) {


            SaveReport s = new SaveReport();

            switch (filterName) {
                case "allData":

                    title = $"report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToCsv(allObj, MSBeverageRecordApp, title, filterName);
                    break;
                case "category":
                    title = $"category report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToCsv(catObj, MSBeverageRecordApp2, title, filterName);
                    break;
                case "manufacturer":
                    title = $"manufacturers    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToCsv(manuObj, MSBeverageRecordApp3, title, filterName);
                    break;
                case "location":
                    title = $"location report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToCsv(locObj, MSBeverageRecordApp4, title, filterName);

                    break;
                case "totalValue":
                    title = $"cost report    {DateTime.UtcNow.ToString("d")}";
                    s.ExportToCsv(totalObj, MSBeverageRecordApp5, title, filterName);
                    break;
            }

        }
        #endregion

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

        public class RootObject {
            public int id { get; set; }
            public List<Records> Items { get; set; }
            public List<TotalCostData> CostItems { get; set; }
        }
        public class TotalCostData {
            public string categoryName { get; set; }
            public decimal cost { get; set; }
        }




        #region Tab category filters


        public List<Records> FilterHotspotRecordsCategory(List<Records> records, ComboBox filter) {
            totalBox = 0.0m;
            List<Records> filteredRecords = new List<Records>();

            if (filter == null || filter.SelectedItem == null) {
                return records;
            }

            if (filter.SelectedItem.ToString() == "All Categories") {
                foreach (Records record in records) {

                    totalBox += record.cost;

                }
                lblCost.Content = $"cost: {totalBox.ToString("C")}";
                return records;
            }

                string selectedCategory = filter.SelectedItem.ToString();
            Console.WriteLine($"Filtering records by category: {selectedCategory}");

            foreach (var record in records) {
                if (record.categoryName == selectedCategory && record.is_deleted == 0) {
                    filteredRecords.Add(record);
                    Console.WriteLine($"Adding record: {record}");
                    totalBox += record.cost;
                }
            }

            //add total cost to xaml element
            lblCost.Content = $"cost: {totalBox.ToString("C")}";
            lblCost.Visibility = Visibility.Visible;
            return filteredRecords;
        }

        public void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var originalRecords = deserializeObject.Items;

            var recordsCopy = new List<Records>(originalRecords);

            var filteredRecords = FilterHotspotRecordsCategory(recordsCopy, FilterCategory);

            MSBeverageRecordApp2.ItemsSource = filteredRecords;

            catGrid.ItemsSource = new List<Records>(filteredRecords);

            catObj.Items = new List<Records>(filteredRecords);

        }


        private void CreateCategoryFilterItems(RootObject list) {
            bool contains = false;

            FilterCategory.Items.Add("All Categories");

            for (int index = 0; index < list.Items.Count; index++) {
                contains = false;
                for (int itemIndex = 0; itemIndex < FilterCategory.Items.Count; itemIndex++)
                    if (FilterCategory.Items[itemIndex].ToString() == list.Items[index].categoryName) {
                        contains = true;
                    }

                if (contains == false) {
                    FilterCategory.Items.Add(list.Items[index].categoryName);
                }
            }

        }


        #endregion Tab category


        #region Tab manufacturer


        public List<Records> FilterHotspotRecordsManufacturer(List<Records> records, ComboBox filter) {

            totalBox = 0.0m;

            List<Records> filteredRecords = new List<Records>();

            if (filter == null || filter.SelectedItem == null) {
                return records;
            }

            if (filter.SelectedItem.ToString() == "All Manufacturers") {
                //lblCost.Visibility = Visibility.Collapsed;
                foreach (Records record in records) {

                    totalBox += record.cost;

                }
                lblCost.Content = $"cost: {totalBox.ToString("C")}";

                return records;
            }

            string selectedManufacturer = filter.SelectedItem.ToString();

            foreach (Records record in records) {
                if (record.companyName == selectedManufacturer && record.is_deleted == 0) {
                    filteredRecords.Add(record);
                    totalBox += record.cost;
                }
            }
            lblCost.Content = $"cost: {totalBox.ToString("C")}";
            lblCost.Visibility = Visibility.Visible;
            return filteredRecords;
        }



        public void Filter_SelectionChangedManufacturer(object sender, SelectionChangedEventArgs e) {
            var originalRecords = deserializeObject.Items;

            var recordsCopy = new List<Records>(originalRecords);

            var filteredRecords = FilterHotspotRecordsManufacturer(recordsCopy, FilterManufacturer);

            MSBeverageRecordApp3.ItemsSource = filteredRecords;

            manuGrid.ItemsSource = new List<Records>(filteredRecords);

            manuObj.Items = new List<Records>(filteredRecords);
        }


        private void CreateManufacturerFilterItems(RootObject list) {
            bool contains = false;

            FilterManufacturer.Items.Add("All Manufacturers");

            for (int index = 0; index < list.Items.Count; index++) {
                contains = false;
                for (int itemIndex = 0; itemIndex < FilterManufacturer.Items.Count; itemIndex++)
                    if (FilterManufacturer.Items[itemIndex].ToString() == list.Items[index].companyName) {
                        contains = true;
                    }

                if (contains == false) {
                    FilterManufacturer.Items.Add(list.Items[index].companyName);
                }
            }


        }


        #endregion Tab manufacturer


        #region Tab location

        public List<Records> FilterHotspotRecordsLocation(List<Records> records, ComboBox filter) {
            // Reset totalBox to 0 before calculating

                totalBox = 0.0m; 
            

            List<Records> filteredRecords = new List<Records>();

            if (filter == null || filter.SelectedItem == null) {
                return records;
            }

            if (filter.SelectedItem.ToString() == "All Locations" ) {

                foreach (Records record in records) {

                    totalBox += record.cost;

                }
                lblCost.Content = $"cost: {totalBox.ToString("C")}";
                //lblCost.Visibility = Visibility.Visible;
                return records;
            }

            string selectedLocation = filter.SelectedItem.ToString();
            Console.WriteLine($"Filtering records by location: {selectedLocation}");

            foreach (Records record in records) {
                if (record.locationName == selectedLocation && record.is_deleted == 0) {
                    filteredRecords.Add(record);
                    Console.WriteLine($"Adding record: {record}");
                    totalBox += record.cost;
                }
            }
            lblCost.Content = $"cost: {totalBox.ToString("C")}";
            //lblCost.Visibility = Visibility.Visible;
            return filteredRecords;
        }

        public void Filter_SelectionChangedLocation(object sender, SelectionChangedEventArgs e) {
            var originalRecords = deserializeObject.Items;
            var recordsCopy = new List<Records>(originalRecords);

            if (isSubLocationFiltering) {
                var filteredRecords = FilterHotspotRecordsLocation(recordsCopy, FilterLocation);
                MSBeverageRecordApp4.ItemsSource = filteredRecords;
                locGrid.ItemsSource = new List<Records>(filteredRecords);
                locObj.Items = new List<Records>(filteredRecords);

                FilterSubLocation.Text = "Select Sub-Location";
            }

            // Reset the sublocation filtering flag
            isSubLocationFiltering = false;

            // Get the selected location
            string selectedLocation = FilterLocation.SelectedItem?.ToString();

            filteredRecords = FilterHotspotRecordsLocation(recordsCopy, FilterLocation);
            MSBeverageRecordApp4.ItemsSource = filteredRecords;
            locGrid.ItemsSource = new List<Records>(filteredRecords);
            locObj.Items = new List<Records>(filteredRecords);

            FilterSubLocation.Text = "Select Sub-Location";

            if (selectedLocation == "All Locations") {
                FilterSubLocation.Visibility = Visibility.Collapsed;
                MSBeverageRecordApp4.ItemsSource = deserializeObject.Items;
            } else {
                FilterSubLocation.Visibility = Visibility.Visible;
                CreateSubLocationFilterItems(deserializeObject, selectedLocation);
                filteredRecords = FilterHotspotRecordsLocation(recordsCopy, FilterLocation);
                MSBeverageRecordApp4.ItemsSource = filteredRecords;
                locGrid.ItemsSource = new List<Records>(filteredRecords);
                locObj.Items = new List<Records>(filteredRecords);
            }
        }

        private void CreateLocationFilterItems(RootObject list) {
            bool contains = false;

            FilterLocation.Items.Add("All Locations");

            for (int index = 0; index < list.Items.Count; index++) {
                contains = false;
                for (int itemIndex = 0; itemIndex < FilterLocation.Items.Count; itemIndex++)
                    if (FilterLocation.Items[itemIndex].ToString() == list.Items[index].locationName) {
                        contains = true;
                    }

                if (contains == false) {
                    FilterLocation.Items.Add(list.Items[index].locationName);
                }
            }

        }


        #endregion Tab location

        #region subLoc
        public List<Records> FilterHotspotRecordsSubLocation(List<Records> records, string selectedLocation, string selectedSubLocation) {
            // Reset totalBox to 0 before calculating
            if (isSubLocationFiltering) {
                totalBox = 0.0m;

                List<Records> filteredRecords = new List<Records>();

                foreach (Records record in records) {
                    // Only add records that match the selected location and sub-location
                    if (record.locationName == selectedLocation && record.sub_location == selectedSubLocation && record.is_deleted == 0) {
                        filteredRecords.Add(record);
                        totalBox += record.cost;
                    } else if (record.locationName == selectedLocation && selectedSubLocation == "All Sub-Locations" && record.is_deleted == 0) {
                        filteredRecords.Add(record);
                        totalBox += record.cost;
                    }
                }

                lblCost.Content = $"cost: {totalBox.ToString("C")}";
                lblCost.Visibility = Visibility.Visible;
                return filteredRecords;
            } else {
                return records;
            }
        }


        public void FilterSubLocation_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            isSubLocationFiltering = true;
            var originalRecords = deserializeObject.Items;
            var recordsCopy = new List<Records>(originalRecords);

            string selectedLocation = FilterLocation.SelectedItem?.ToString();

            string selectedSubLocation = FilterSubLocation.SelectedItem?.ToString();

            var filteredRecords = FilterHotspotRecordsSubLocation(recordsCopy, selectedLocation, selectedSubLocation);
            MSBeverageRecordApp4.ItemsSource = filteredRecords;
            s_locGrid.ItemsSource = new List<Records>(filteredRecords);
            s_locObj.Items = new List<Records>(filteredRecords);

            //isSubLocationFiltering = false; 
        }

        private void CreateSubLocationFilterItems(RootObject list, string selectedLocation) {
            bool contains = false;

            FilterSubLocation.Items.Clear();
            FilterSubLocation.Items.Add("All Sub-Locations");
            for (int index = 0; index < list.Items.Count; index++) {
                if (selectedLocation != null && list.Items[index].locationName != selectedLocation) {
                    continue;
                }

                contains = false;
                for (int itemIndex = 0; itemIndex < FilterSubLocation.Items.Count; itemIndex++) {
                    // Check if the sub-location already exists in the combo box
                    if (FilterSubLocation.Items[itemIndex].ToString() == list.Items[index].sub_location) {
                        // Check if the parent location matches the selected location
                        if (list.Items[index].locationName == selectedLocation) {
                            contains = true;
                            break;
                        }
                    }
                }

                if (!contains && list.Items[index].is_deleted != 1) {
                    FilterSubLocation.Items.Add(list.Items[index].sub_location);
                }
            }
        } 
        #endregion

        #region Total Costs
        //by company
        public static List<TotalCostData> CreateCostReport(RootObject list) {
            List<TotalCostData> data = new List<TotalCostData>();
            if (list.Items == null || list.Items.Count == 0) {
                return data;
            }

            // Dictionary to store category names and their corresponding total costs
            Dictionary<string, decimal> categoryCostMap = new Dictionary<string, decimal>();

            decimal totalCosts = 0;

            for (int index = 0; index < list.Items.Count; index++) {
                if (list.Items[index].is_deleted == 0) {
                    string categoryName = list.Items[index].categoryName;
                    decimal cost = list.Items[index].cost;

                    // Add or update the category cost in the dictionary
                    if (categoryCostMap.ContainsKey(categoryName)) {
                        categoryCostMap[categoryName] += cost;
                    } else {
                        categoryCostMap[categoryName] = cost;
                    }

                    // Add to the total cost
                    totalCosts += cost;
                }
            }

            // Convert dictionary to TotalCostData list
            foreach (var category in categoryCostMap) {
                data.Add(new TotalCostData {
                    categoryName = category.Key,
                    cost = category.Value
                });
            }

            // Add total cost entry
            data.Add(new TotalCostData {
                categoryName = "Total Cost",
                cost = totalCosts
            });

            return data;
        }
        #endregion

        #region tabcontrol 
        //setting grid data and applying filters based on current tab
        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {

 
            if (e.RemovedItems.Contains(xalldata)) {
                MSBeverageRecordApp.Sorting -= MSBeverageRecordApp_Sorting; // Remove sorting event handler
                MSBeverageRecordApp.ItemsSource = null; // Clear ItemsSource
            }

            if (e.RemovedItems.Contains(xcategory)) {
                MSBeverageRecordApp2.Sorting -= MSBeverageRecordApp_Sorting; // Remove sorting event handler
                MSBeverageRecordApp2.ItemsSource = null; // Clear ItemsSource
            }
            if (e.RemovedItems.Contains(xmanufacturer)) {
                MSBeverageRecordApp3.Sorting -= MSBeverageRecordApp_Sorting; // Remove sorting event handler
                MSBeverageRecordApp3.ItemsSource = null; // Clear ItemsSource
            }
            if (e.RemovedItems.Contains(xlocation)) {
                MSBeverageRecordApp4.Sorting -= MSBeverageRecordApp_Sorting; // Remove sorting event handler
                MSBeverageRecordApp4.ItemsSource = null; // Clear ItemsSource
            }

            //lblCost.Visibility = Visibility.Visible;
            if (xalldata.IsSelected) {
                if (filterName != "allData") {
                    totalBox = 0.0m;
                    CostTotal(deserializeObject);
                }
                filterName = "allData";
                MSBeverageRecordApp.ItemsSource = allObj.Items;
                MSBeverageRecordApp.Sorting += MSBeverageRecordApp_Sorting;


            }
            if (xcategory.IsSelected) {
                filterName = "category";
                var filteredCategoryRecords = FilterHotspotRecordsCategory(deserializeObject.Items, FilterCategory);
                MSBeverageRecordApp2.ItemsSource = filteredCategoryRecords;
                catGrid.ItemsSource = filteredCategoryRecords;
                catObj.Items = filteredCategoryRecords;
                MSBeverageRecordApp2.Sorting += MSBeverageRecordApp_Sorting;
            }
            if (xmanufacturer.IsSelected) {
                filterName = "manufacturer";
                var filteredManufacturerRecords = FilterHotspotRecordsManufacturer(deserializeObject.Items, FilterManufacturer);
                MSBeverageRecordApp3.ItemsSource = filteredManufacturerRecords;
                manuGrid.ItemsSource = filteredManufacturerRecords;
                manuObj.Items = filteredManufacturerRecords;

                MSBeverageRecordApp3.Sorting += MSBeverageRecordApp_Sorting;


            }
            if (xlocation.IsSelected) {
                filterName = "location";
                if (!isSubLocationFiltering) {
                    var filteredLocationRecords = FilterHotspotRecordsLocation(deserializeObject.Items, FilterLocation);
                    MSBeverageRecordApp4.ItemsSource = filteredLocationRecords;
                    locGrid.ItemsSource = filteredLocationRecords;
                    locObj.Items = filteredLocationRecords;
                    MSBeverageRecordApp4.Sorting += MSBeverageRecordApp_Sorting;

                }
            }
            if (xtotalvalue.IsSelected && deserializeObject.Items.Count > 0) {
                filterName = "totalValue";
                MSBeverageRecordApp5.ItemsSource = CreateCostReport(deserializeObject);
                totalGrid.ItemsSource = CreateCostReport(deserializeObject);
                totalObj.CostItems = (List<TotalCostData>)totalGrid.ItemsSource;
                //lblCost.Visibility = Visibility.Collapsed;
            }
        }
        #endregion tabcontrol testing

        #region sub combobox search N/A


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
                MSBeverageRecordApp3.Items.Filter = null;
                txtSearchPlaceholder.Visibility = System.Windows.Visibility.Visible;
            } else {
                MSBeverageRecordApp3.Items.Filter = GetFilter();
                txtSearchPlaceholder.Visibility = System.Windows.Visibility.Hidden;

            }
        }


        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            MSBeverageRecordApp3.Items.Filter = GetFilter();

            switch (Filterby.SelectedItem as string) {
                case "Category":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Category";
                    foreach (var categoryItem in deserializeObject.Items) {
                        cboSearch.Items.Add(categoryItem.categoryName);
                    }
                    break;
                case "Manufacturer":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Manufacturer";
                    foreach (var manufacturerItem in deserializeObject.Items) {
                        cboSearch.Items.Add(manufacturerItem.companyName);
                    }
                    break;
                case "Location":
                    clrSearch.Visibility = Visibility.Visible;
                    cboSearch.Items.Clear();
                    cboSearch.Visibility = Visibility.Visible;
                    cboSearch.Text = "Select Location";
                    foreach (var locationItem in deserializeObject.Items) {
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
                        MSBeverageRecordApp3.Items.Filter = item =>
                            ((Records)item).categoryName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "Manufacturer":
                        MSBeverageRecordApp3.Items.Filter = item =>
                            ((Records)item).companyName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "Location":
                        MSBeverageRecordApp3.Items.Filter = item =>
                            ((Records)item).locationName.Equals(selected, StringComparison.OrdinalIgnoreCase);
                        break;
                }

            }
        }

        private void clrSearch_Click(object sender, RoutedEventArgs e) {
            try {
                MSBeverageRecordApp3.Items.Filter = null;

                cboSearch.SelectedItem = null;
                Filterby.SelectedItem = "";
                MSBeverageRecordApp3.ItemsSource = deserializeObject.Items;
                MSBeverageRecordApp3.Items.Refresh();
            } catch (Exception ex) {
                Console.WriteLine("Error in clrSearch_Click: " + ex.Message);
            }
            Filterby.Text = "Search By";
            clrSearch.Visibility = Visibility.Hidden;
            cboSearch.Visibility = Visibility.Hidden;
        }








        #endregion sub combobox search

        private void MSBeverageRecordApp_Sorting(object sender, DataGridSortingEventArgs e) {
            DataGrid dataGrid = sender as DataGrid;

            // Ensure items are of type Records
            List<Records> sortedList = new List<Records>();

            foreach (var item in dataGrid.Items) {
                if (item is Records record) {
                    sortedList.Add(record);
                } else {
                    // Handle unexpected types or null items if necessary
                    // Example: continue; // Skip or log unexpected items
                }

                // Determine the new sort direction
                ListSortDirection newDirection = e.Column.SortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

                // Sort the list based on the clicked column and direction
                if (newDirection == ListSortDirection.Ascending) {
                    sortedList = sortedList.OrderBy(r => r.GetType().GetProperty(e.Column.SortMemberPath).GetValue(r)).ToList();
                } else {
                    sortedList = sortedList.OrderByDescending(r => r.GetType().GetProperty(e.Column.SortMemberPath).GetValue(r)).ToList();
                }

                // Update the DataGrid and filteredRecords list
                e.Column.SortDirection = newDirection;
                dataGrid.ItemsSource = sortedList;
                filteredRecords = sortedList;

                // Update other UI components bound to filteredRecords
                manuObj.Items = new List<Records>(filteredRecords);
                locObj.Items = new List<Records>(filteredRecords);
                catObj.Items = new List<Records>(filteredRecords);
                allObj.Items = new List<Records>(filteredRecords);

                // Prevent the default sort from occurring
                e.Handled = true;
            }
        }



    }//c
}//n

