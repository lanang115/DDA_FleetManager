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
    /// Interaction logic for FuelPurchaseList.xaml
    /// </summary>
    public partial class FuelPurchaseList : MetroWindow
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
        public Booking vehicles;
        /// <summary>
        /// Property: Database connection
        /// </summary>
        private MySqlConnection connection;
        public static ObservableCollection<FuelPurchase> fuelPurchaseList { get; private set; }
        public FuelPurchaseList()
        {
            InitializeComponent();
            InitializeDb();
            ScanStatusKeysInBackground();
            FillFuelPurchaseTable();
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
        /// this is a method to get data from database
        /// </summary>
        /// <param name="searchTerm"></param>
        private void FillFuelPurchaseTable(string searchTerm = "")
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

                    string sql = "SELECT * FROM `nmt_fleet_manager`.`fuel_purchases`";
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
                        fuelPurchaseList = MapDataTableToObservableCollection(dt);
                        FuelPurchaseListView.ItemsSource = fuelPurchaseList;
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
        public ObservableCollection<FuelPurchase> MapDataTableToObservableCollection(DataTable dt)
        {
            ObservableCollection<FuelPurchase> fuelPurchaseList = new ObservableCollection<FuelPurchase>();

            foreach (DataRow row in dt.Rows)
            {
                fuelPurchaseList.Add(
                  new FuelPurchase(int.Parse(row["id"].ToString()))
                  {
                      VId = int.Parse(row["vehicle_id"].ToString()),
                      SelectedVehicle = row["selected_vehicle"].ToString(),
                      FuelQuantity = decimal.Parse(row["fuel_quantity"].ToString()),
                      FuelPrice = decimal.Parse(row["fuel_price"].ToString()),
                      TotalCost = decimal.Parse(row["total_cost"].ToString()),
                  }
                );
            }
            return fuelPurchaseList;
        }
        /// <summary>
        /// this is a event for filterbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillFuelPurchaseTable(FilterTextBox.Text);
        }
        /// <summary>
        /// this is a click event for delete fuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFuelButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button deleteFuelButton = (Button)sender;
            FuelPurchase fuelPurchase = deleteFuelButton.DataContext as FuelPurchase;
            fuelPurchaseList.Remove(fuelPurchase);
            string deleteBookingSQL = "DELETE FROM `nmt_fleet_manager`.`fuel_purchases`" +
                " WHERE `id`='" + fuelPurchase.id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(deleteBookingSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
            }
        }
        /// <summary>
        /// this is a click event for edit fuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editFuelButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button fuelEditButton = sender as Button;
            FuelPurchase fuelPurchase = fuelEditButton.DataContext as FuelPurchase;
            EditFuelPurchases editFuelWindow = new EditFuelPurchases(fuelPurchase.id,fuelPurchase.FuelQuantity, fuelPurchase.FuelPrice,
                fuelPurchase.SelectedVehicle);

            editFuelWindow.Owner = this;
            editFuelWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (editFuelWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Fuel Updated");
                FillFuelPurchaseTable();
                UpdateStatus(50, "Ready...");
            }
            editFuelWindow.Close();
        }
        /// <summary>
        /// this is a click event for booking List Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bookingListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            BookingList bookingList = new BookingList();
            bookingList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for journey list menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void journeyListMenu_Clciked(object sender, RoutedEventArgs e)
        {
            JourneyList journeyList = new JourneyList();
            journeyList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for car list menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            CarList carList = new CarList();
            carList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for serviceMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceMenu_Clicked(object sender, RoutedEventArgs e)
        {
            ServiceList serviceList = new ServiceList();
            serviceList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for aboutMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutMenu_Clicked(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}
