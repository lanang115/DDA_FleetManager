using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for AddJourneyForm.xaml
    /// </summary>
    public partial class AddJourneyForm : Window
    {
        private int bookingid;
        private int vehicleid;
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
        public AddJourneyForm()
        {
            InitializeComponent();
            InitializeDb();
            HideErrors();
            OpenConnection();
            CloseConnection();
        }
        /// <summary>
        /// this is a constructor to initialize db connection
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
        /// this is a constructor for setting textbox
        /// </summary>
        /// <param name="selectedVehicle"></param>
        /// <param name="id"></param>
        /// <param name="vehicleid"></param>
        public AddJourneyForm(string selectedVehicle, int bookingid, int vehicleid)
        {
            InitializeComponent();
            InitializeDb();
            HideErrors();
            JourneySelectedVehicleTextBox.Text = selectedVehicle;
            this.bookingid = bookingid;
            this.vehicleid = vehicleid;
        }

        /// <summary>
        /// Hide error messages
        /// </summary>
        private void HideErrors()
        {
            LabelEndedAtDate.Visibility = Visibility.Hidden;
            LabelStartAtDate.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Open connection to the MySQL Database
        /// </summary>
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact Administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Close the MySQL Database connection
        /// </summary>
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// this is a click event for addJourney
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addJourneyButton_Clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
            int bookingId = bookingid;
            int vehicleId = vehicleid;
            string selectedVehicle = JourneySelectedVehicleTextBox.Text;
            string startOdometer = StartOdometerJourneyTextBox.Text;
            string endOdometer = EndedOdometerJourneyTextBox.Text;
            string startJourneyDate = JourneyStartAtDate.Text;
            string endJourneyDate = JourneyEndedAtDate.Text;
            string journeyFrom = JourneyFromTextBox.Text;
            string journeyTo = JourneyToTextBox.Text;
            bool validStartDate = ValidateStartDate(startJourneyDate);
            bool validEndDate = ValidateEndDate(endJourneyDate);

            if (!validStartDate)
            {
                // TODO: Display error to the user
                LabelStartAtDate.Content = "cannot be null";
                LabelStartAtDate.Visibility = Visibility.Visible;
            }
            else if (!validEndDate)
            {
                // TODO: Display error to the user
                LabelEndedAtDate.Content = "cannot be null";
                LabelEndedAtDate.Visibility = Visibility.Visible;
            }
            if (validStartDate && validEndDate)
            {
                AddJourneyToDB(bookingId, vehicleId, selectedVehicle,
                    startOdometer, endOdometer, startJourneyDate, endJourneyDate, journeyFrom, journeyTo);
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// this is a bool to check if its null or not
        /// </summary>
        /// <param name="regisId"></param>
        /// <returns></returns>
        private bool ValidateStartDate(string startJourneyDate)
        {
            if (startJourneyDate == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// this is a bool to check if its null or not
        /// </summary>
        /// <param name="carManufacture"></param>
        /// <returns></returns>
        private bool ValidateEndDate(string endJourneyDate)
        {
            if (endJourneyDate == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// this is a click event for close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a method to add journey to database
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="selectedVehicle"></param>
        /// <param name="startOdometer"></param>
        /// <param name="endOdometer"></param>
        /// <param name="startJourneyDate"></param>
        /// <param name="endJourneyDate"></param>
        /// <param name="journeyFrom"></param>
        /// <param name="journeyTo"></param>
        /// <returns></returns>
        private bool AddJourneyToDB(int bookingId ,int vehicleId, string selectedVehicle, string startOdometer,
            string endOdometer, string startJourneyDate, string endJourneyDate, string journeyFrom, string journeyTo)
        {
            string addJourneySQL = "INSERT INTO `nmt_fleet_manager`.`journeys`(booking_id,vehicle_id, selected_vehicle, start_odometer, end_odometer, journey_start_at, journey_end_at, journey_from, journey_to) "
                + " VALUES ( '" + bookingId + "','" + vehicleId + "', '" + selectedVehicle + "', '" + startOdometer + "', '" + endOdometer + "', STR_TO_DATE( '" + startJourneyDate + "', '%d/%m/%Y %H:%i:%s')," +
                " STR_TO_DATE( '" + endJourneyDate + "', '%d/%m/%Y %H:%i:%s'), '" + journeyFrom + "','" + journeyTo + "' )";

            using (MySqlCommand cmdSel = new MySqlCommand(addJourneySQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.SelectCommand = cmdSel;
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows == 0;
            }
        }
    }
}
