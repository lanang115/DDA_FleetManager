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
    /// Interaction logic for BookingList.xaml
    /// </summary>
    public partial class BookingList : MetroWindow
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
        public static ObservableCollection<Booking> bookingList { get; private set; }
        /// <summary>
        /// this is a constructor for this window
        /// </summary>
        public BookingList()
        {
            InitializeComponent();
            FillBookingTable();
            ScanStatusKeysInBackground();
            InitializeDb();
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
        private void FillBookingTable(string searchTerm = "")
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

                    string sql = "SELECT * FROM `nmt_fleet_manager`.`bookings`";
                    if (!searchTerm.Equals(""))
                    {
                        sql += " WHERE customer_name LIKE '%" + searchTerm + "%'";
                    }
                    MessageTextBox.Text = sql;

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);

                        da.Fill(dt);
                        bookingList = MapDataTableToObservableCollection(dt);
                        BookingListView.ItemsSource = bookingList;
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
        public ObservableCollection<Booking> MapDataTableToObservableCollection(DataTable dt)
        {
            ObservableCollection<Booking> bookingList = new ObservableCollection<Booking>();

            foreach (DataRow row in dt.Rows)
            {
                bookingList.Add(
                  new Booking(int.Parse(row["id"].ToString()))
                  {
                      Vehicleid = int.Parse(row["vehicle_id"].ToString()),
                      CustomerName = row["customer_name"].ToString(),
                      SelectedVehicle = row["selected_vehicle"].ToString(),
                      RentalType = (BookingType)Enum.Parse(typeof(BookingType),row["booking_type"].ToString()),
                      StartOdometer= int.Parse(row["start_odometer"].ToString()),
                      StartRentDate = DateTime.Parse(row["start_at"].ToString()),
                      EndRentDate = DateTime.Parse(row["end_at"].ToString()),
                  }
                );
            }
            return bookingList;
        }
        /// <summary>
        /// this is a method for filterbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillBookingTable(FilterTextBox.Text);
        }
        /// <summary>
        /// this is a click event to delete booking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteBooking_Clicked(object sender, RoutedEventArgs e)
        {
            Button deleteBookingButton = (Button)sender;
            Booking booking = deleteBookingButton.DataContext as Booking;
            bookingList.Remove(booking);
            string deleteBookingSQL = "DELETE FROM `nmt_fleet_manager`.`bookings`" +
                " WHERE `id`='" + booking.id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(deleteBookingSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
            }
        }
        /// <summary>
        /// this is a click event for edit Booking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBooking_Clicked(object sender, RoutedEventArgs e)
        {
            Button bookingEditButton = sender as Button;
            Booking booking = bookingEditButton.DataContext as Booking;
            EditBooking editBookingWindow = new EditBooking(booking.id,booking.CustomerName, booking.StartOdometer, booking.StartRentDate, booking.EndRentDate
                , booking.RentalType, booking.SelectedVehicle);

            editBookingWindow.Owner = this;
            editBookingWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (editBookingWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Booking Updated");
                FillBookingTable();
                UpdateStatus(50, "Ready...");
            }
            editBookingWindow.Close();
        }
        /// <summary>
        /// this is a click event for addjourney
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addJourneyButton_Clicked(object sender, RoutedEventArgs e)
        {
            //int newId = new int();
            Button addJourneyButton = (Button)sender;
            Booking bookingItem = addJourneyButton.DataContext as Booking;
            AddJourneyForm bookingFormWindow = new AddJourneyForm(bookingItem.SelectedVehicle,
                bookingItem.id, bookingItem.Vehicleid);
            
            bookingFormWindow.Owner = this;
            bookingFormWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (bookingFormWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Booking Added");
                FillBookingTable();
                UpdateStatus(50, "Ready...");
            }
            bookingFormWindow.Close();
        }
        /// <summary>
        /// this is a click event for carListMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            CarList carList = new CarList();
            carList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for journey list menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void journeyListMenu_CLicked(object sender, RoutedEventArgs e)
        {
            JourneyList journeyList = new JourneyList();
            journeyList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for addFuel Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFuelButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button addFuelButton = (Button)sender;
            Booking bookingItem = addFuelButton.DataContext as Booking;
            AddFuelPurchase addFuelWindow = new AddFuelPurchase(bookingItem.SelectedVehicle, bookingItem.Vehicleid);

            addFuelWindow.Owner = this;
            addFuelWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (addFuelWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Booking Added");
                FillBookingTable();
                UpdateStatus(50, "Ready...");
            }
            addFuelWindow.Close();
        }
        /// <summary>
        /// this is a click event for FuelPurchase Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fuelPurchaseMenu_Clicked(object sender, RoutedEventArgs e)
        {
            FuelPurchaseList fuelPurchaseList = new FuelPurchaseList();
            fuelPurchaseList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for service menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceMenu_Clicked(object sender, RoutedEventArgs e)
        {
            ServiceList serviceList = new ServiceList();
            serviceList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for about menu
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
