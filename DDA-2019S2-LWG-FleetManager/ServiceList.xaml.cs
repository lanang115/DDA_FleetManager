using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DDA_2019S2_LWG_FleetManager
{
    /// <summary>
    /// Interaction logic for ServiceList.xaml
    /// </summary>
    public partial class ServiceList : MetroWindow
    {
        private string server;
        private string database;
        private string db_user;
        private string password;
        private int port;
        /// <summary>
        /// DSN String (Data Source Name)
        /// </summary>
        private string dsnString;

        /// <summary>
        /// Property: Database connection
        /// </summary>
        private MySqlConnection connection;
        public Vehicle vehicle;
        public static ObservableCollection<Service> serviceList { get; private set; }
        public ServiceList()
        {
            InitializeComponent();
            InitializeDb();
            ScanStatusKeysInBackground();
            FillServiceTable();
        }
        /// <summary>
        /// this is a method to initialize database
        /// </summary>
        private void InitializeDb()
        {
            server = "localhost";
            database = "nmt_fleet_manager";
            db_user = "nmt_fleet_manager";
            password = "Password1";
            port = 3306;

            dsnString = "SERVER=" + server + ";"
            + "PORT=" + port + ";"
            + "DATABASE=" + database + ";"
            + "UID=" + db_user + ";"
            + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(dsnString);
        }

        /// <summary>
        /// this is a method to check toggled key on your keyboard
        /// </summary>
        public void CheckKeyStatus()
        {
            var isNumLockToggled = Keyboard.IsKeyToggled(Key.NumLock); // variable for NumLock
            var isScrollLockToggled = Keyboard.IsKeyToggled(Key.Scroll); // variable for ScrollLock
            var isCapsLockToggled = Keyboard.IsKeyToggled(Key.CapsLock); // variable for CapsLock

            if (isNumLockToggled)
            {
                // if the NumLock is toggled on keyboard
                // The text color will change into red
                NumLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the NumLock on your keyboard
                // The text color will change into gray
                NumLockStatus.Foreground = Brushes.Gray;
            }

            if (isCapsLockToggled)
            {
                // if the CapsLock is toggled on keyboard
                // The text color will change into red
                CapsLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the CapsLock on your keyboard
                // The text color will change into gray
                CapsLockStatus.Foreground = Brushes.Gray;
            }

            if (isScrollLockToggled)
            {
                // if the ScrollLock is toggled on keyboard
                // The text color will change into red
                ScrollLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                // if you untoggle the ScrollLock on your keyboard
                // The text color will change into gray
                ScrollLockStatus.Foreground = Brushes.Gray;
            }
        }
        /// <summary>
        /// to check the toggled key using await
        /// </summary>
        /// <returns></returns>
        private async Task ScanStatusKeysInBackground()
        {
            while (true)
            {
                CheckKeyStatus();
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// this is a method to fillServiceTable
        /// </summary>
        /// <param name="searchTerm"></param>
        private void FillServiceTable(string searchTerm = "")
        {

            // Data Source String (Data Source Name)
            string dsnString = "server=localhost;" +
                "user=nmt_fleet_manager;" +
                "database=nmt_fleet_manager;" +
                "port=3306;" +
                "password=Password1";
            MessageTextBlock.Background = this.FindResource
                (SystemColors.ControlLightBrushKey) as Brush;
            MessageTextBlock.Foreground = Brushes.Black;

            try
            {
                // Perform database operations

                UpdateStatus(500, "Processing...");

                using (MySqlConnection connection = new MySqlConnection(dsnString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM `nmt_fleet_manager`.`services`";
                    if (!searchTerm.Equals(""))
                    {
                        sql += " WHERE selected_vehicle LIKE '%" + searchTerm + "%'";
                    }
                    MessageTextBox.Text = sql;

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);

                        da.Fill(dt);
                        serviceList = MapDataTableToObservableCollection(dt);
                        ServiceListView.ItemsSource = serviceList;
                        //VehicleListView.ItemsSource = dt.DefaultView;
                    }
                    //refreshVehicleList();
                    connection.Close();
                    UpdateStatus(500, "Ready...");
                }

            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: " + ex.ToString() + "\n");
                MessageTextBlock.Text = "Unable to connect Database...";
                MessageTextBlock.Background = Brushes.Red;
                MessageTextBlock.Foreground = Brushes.White;

            }
        }
        /// <summary>
        /// this is a method to updateStatus
        /// </summary>
        /// <param name="delayperiod"></param>
        /// <param name="MessageToShow"></param>
        private async void UpdateStatus(int delayperiod = 1500, string
            MessageToShow = "")
        {
            MessageTextBlock.Text = MessageToShow;
            await Task.Delay(delayperiod);
        }

        /// <summary>
        /// mapping data from data table into observablecollection
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public ObservableCollection<Service> MapDataTableToObservableCollection(DataTable dt)
        {
            ObservableCollection<Service> serviceList = new ObservableCollection<Service>();

            foreach (DataRow row in dt.Rows)
            {
                serviceList.Add(
                  new Service(int.Parse(row["id"].ToString()))
                  {
                      vehicleId = int.Parse(row["vehicle_id"].ToString()),
                      SelectedVehicle = row["selected_vehicle"].ToString(),
                      ServiceOdometer = int.Parse(row["service_odometer"].ToString()),
                      ServiceDate = DateTime.Parse(row["service_at"].ToString()),
                  }
                );
            }
            return serviceList;
        }
        /// <summary>
        /// this is a method for filtertextbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTextBox_Changed(object sender, TextChangedEventArgs e)
        {
            FillServiceTable(FilterTextBox.Text);
        }
        /// <summary>
        /// this is a click event for deleteService button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteService_Clicked(object sender, RoutedEventArgs e)
        {
            Button deleteServiceButton = (Button)sender;
            Service service = deleteServiceButton.DataContext as Service;
            serviceList.Remove(service);
            string deleteVehicleSQL = "DELETE FROM `nmt_fleet_manager`.`services`" +
                " WHERE `vehicle_id`='" + service.vehicleId + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(deleteVehicleSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
            }
        }
        /// <summary>
        /// this is a click event for edit service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditServiceButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button serviceEditButton = sender as Button;
            Service service = serviceEditButton.DataContext as Service;
            EditService editServiceWindow = new EditService(service.id, service.SelectedVehicle, service.ServiceOdometer, service.ServiceDate);

            editServiceWindow.Owner = this;
            editServiceWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (editServiceWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Vehicle Updated");
                FillServiceTable();
                UpdateStatus(50, "Ready...");
            }
            editServiceWindow.Close();
        }
        /// <summary>
        /// this is a  click event for booking list menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bookingListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            BookingList bookingList = new BookingList();
            bookingList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for journeyList menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void journeyList_Clicked(object sender, RoutedEventArgs e)
        {
            JourneyList journeyList = new JourneyList();
            journeyList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for fuel purchase menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fuelPurchasesMenu_Clicked(object sender, RoutedEventArgs e)
        {
            FuelPurchaseList fuelPurchase = new FuelPurchaseList();
            fuelPurchase.ShowDialog();
        }
        /// <summary>
        /// this is a click event for car List Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            CarList carList = new CarList();
            carList.ShowDialog();
        }
        /// <summary>
        /// this is a click event aboutMenu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutMenu_Clicked(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}
