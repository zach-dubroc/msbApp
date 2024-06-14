using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace MSBeverageRecordApp {

    public partial class CategoryTable : Page {
        class PostResponse {
            public int Id { get; set; }
        }

        class PostCategory {
            public string categoryName { get; set; }
        }

        public class Category {
            public int id { get; set; }
            public string categoryName { get; set; }
        }

        private List<Category> existingCategories;

        public CategoryTable() {
            InitializeComponent();
            LoadCategories();
        }

        private async void LoadCategories() {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri("http://localhost:4001/");
                client.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("api/category");
                if (response.IsSuccessStatusCode) {
                    var dataObjects = await response.Content.ReadAsStringAsync();
                    existingCategories = JsonSerializer.Deserialize<List<Category>>(dataObjects);
                    CreateCategoryComboBox(existingCategories);
                } else {
                    existingCategories = new List<Category>();
                }
            }
        }

        private async void Category_Button_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(txtCategory.Text)) {
                MessageBox.Show("Please enter a value in the category.");
            } else {
                string input = txtCategory.Text.Trim().ToUpper();
                txtCategory.Text = "";

                bool categoryExists = existingCategories.Exists(c => c.categoryName == input);
                if (categoryExists) {
                    MessageBox.Show("Category already exists. Please enter a new category.");
                    return;
                }

                var postData = new PostCategory {
                    categoryName = input
                };

                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri("http://localhost:4001/");

                    var json = JsonSerializer.Serialize(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/category/categorycreate/", content);

                    if (response.IsSuccessStatusCode) {
                        MessageBox.Show("New Category Created");


                    } else {
                        MessageBox.Show("Failed to create category. Please try again.");
                    }

                    this.NavigationService.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
                }
            }
        }

        private void CreateCategoryComboBox(List<Category> list) {
            cboCat.Items.Clear();
            foreach (var category in list) {
                if (!string.IsNullOrEmpty(category.categoryName)) {
                    cboCat.Items.Add(category.categoryName);
                }
            }
        }
    }
}
