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
    /// Interaction logic for JourneyList.xaml
    /// </summary>
    public partial class JourneyList : Window
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
        private MySqlConnection connection;
        public static ObservableCollection<Journey> journeyList { get; private set; }
        public JourneyList()
        {
            InitializeComponent();
            ScanStatusKeysInBackground();
            FillJourneyTable();
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
        private void FillJourneyTable(string searchTerm = "")
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

                    string sql = "SELECT * FROM `nmt_fleet_manager`.`journeys`";
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
                        journeyList = MapDataTableToObservableCollection(dt);
                        JourneyListView.ItemsSource = journeyList;
                    }
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
        /// mapping the value from database into an observable collection
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public ObservableCollection<Journey> MapDataTableToObservableCollection(DataTable dt)
        {
            ObservableCollection<Journey> journeyList = new ObservableCollection<Journey>();

            foreach (DataRow row in dt.Rows)
            {
                journeyList.Add(
                  new Journey(int.Parse(row["id"].ToString()))
                  {
                      BookingID = int.Parse(row["booking_id"].ToString()),
                      VehicleID = int.Parse(row["vehicle_id"].ToString()),
                      selectedVehicle = row["selected_vehicle"].ToString(),
                      JourneyStartAt = DateTime.Parse(row["journey_start_at"].ToString()),
                      JourneyEndedAt = DateTime.Parse(row["journey_end_at"].ToString()),
                      StartOdometer = int.Parse(row["start_odometer"].ToString()),
                      EndOdometer = int.Parse(row["end_odometer"].ToString()),
                      JourneyFrom = row["journey_from"].ToString(),
                      JourneyTo = row["journey_to"].ToString(),
                  }
                );
            }
            return journeyList;
        }
        /// <summary>
        /// this is a method for filterbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filterbox_textChanged(object sender, TextChangedEventArgs e)
        {
            FillJourneyTable(FilterTextBox.Text);
        }
        /// <summary>
        /// this is a click event for delete journey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteJourney_Clicked(object sender, RoutedEventArgs e)
        {
            Button deleteJourneyButton = (Button)sender;
            Journey journey = deleteJourneyButton.DataContext as Journey;
            journeyList.Remove(journey);
            string deleteJourneySQL = "DELETE FROM `nmt_fleet_manager`.`journeys`" +
                " WHERE `id`='" + journey.id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(deleteJourneySQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
            }
        }
        /// <summary>
        /// this is a click event for editJourney
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditJourney_Clicked(object sender, RoutedEventArgs e)
        {
            Button journeyEditButton = sender as Button;
            Journey journey = journeyEditButton.DataContext as Journey;
            EditJourney editJourneyWindow = new EditJourney(journey.id,journey.JourneyStartAt, journey.JourneyEndedAt, 
                journey.StartOdometer, journey.EndOdometer, journey.JourneyFrom, journey.JourneyTo, journey.selectedVehicle);

            editJourneyWindow.Owner = this;
            editJourneyWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;

            if (editJourneyWindow.ShowDialog() == true)
            {
                UpdateStatus(5000, "Journey Updated");
                FillJourneyTable();
                UpdateStatus(50, "Ready...");
            }
            editJourneyWindow.Close();
        }
        /// <summary>
        /// this is a click event for carlist menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            CarList carList = new CarList();
            carList.ShowDialog();
        }
        /// <summary>
        /// this is a click event for bookingListMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookingListMenu_Clicked(object sender, RoutedEventArgs e)
        {
            BookingList bookingList = new BookingList();
            bookingList.ShowDialog();
        }
    }
}
