using System.Windows;
using System.Windows.Controls;

namespace MSBeverageRecordApp {

    public partial class MenuPage : Page {
        public MenuPage() {
            InitializeComponent();
        }

        #region Button Event Functions
        private void btnAddCategory_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("CategoryTable.xaml", UriKind.Relative));
        }
        private void btnViewReports_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("Reports.xaml", UriKind.Relative));
        }
        private void addRecord(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("CreateRecord.xaml", UriKind.Relative));
        }
        private void btnModifyDeleteRecord_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("CrudWindow.xaml", UriKind.Relative));
        }
        private void btnAddManufacturer_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("AddManufacturer.xaml", UriKind.Relative));
        }
        private void btnAddLocation_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("AddLocation.xaml", UriKind.Relative));
        }
        #endregion
    }
}
