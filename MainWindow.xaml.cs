using System.Windows;


namespace MSBeverageRecordApp {

    /// <summary>
    /// INTERACTION LOGIC FOR MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            //SET MAINFRAME TO OPEN MENU PAGE ON STARTUP
            MainFrame.Content = new MenuPage();
        }//end main window

    }//end class
}//end namespace