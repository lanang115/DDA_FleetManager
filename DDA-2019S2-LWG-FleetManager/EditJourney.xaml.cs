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
    /// Interaction logic for EditJourney.xaml
    /// </summary>
    public partial class EditJourney : Window
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
        private int id;
        public EditJourney()
        {
            InitializeComponent();
            InitializeDb();
            HideErrors();
            OpenConnection();
            CloseConnection();
        }
        /// <summary>
        /// this is a method to initialize the database
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
        /// this is a constructor to set the textbox value
        /// </summary>
        /// <param name="journeyStartAt"></param>
        /// <param name="journeyEndedAt"></param>
        /// <param name="startOdometer"></param>
        /// <param name="endOdometer"></param>
        /// <param name="journeyFrom"></param>
        /// <param name="journeyTo"></param>
        /// <param name="selectedVehicle"></param>
        public EditJourney(int id, DateTime journeyStartAt, DateTime journeyEndedAt, int startOdometer, int endOdometer, string journeyFrom, string journeyTo, string selectedVehicle)
        {
            InitializeComponent();
            HideErrors();
            InitializeDb();
            this.id = id;
            JourneyStartAtDate.Text = journeyStartAt.ToString();
            JourneyEndedAtDate.Text = journeyEndedAt.ToString();
            StartOdometerJourneyTextBox.Text = startOdometer.ToString();
            EndedOdometerJourneyTextBox.Text = endOdometer.ToString();
            JourneyFromTextBox.Text = journeyFrom;
            JourneyToTextBox.Text = journeyTo;
            JourneySelectedVehicleTextBox.Text = selectedVehicle;
        }

        /// <summary>
        /// this is a click event for edit journey save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editSaveChangesForJourney_clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
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
                EditJourneyToDB(id, selectedVehicle,
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
        /// this is a click event ffor edit cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancelEdit_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a method to update the changes for
        /// edit journey
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerName"></param>
        /// <param name="selectedVehicle"></param>
        /// <param name="bookingType"></param>
        /// <param name="startOdometer"></param>
        /// <param name="startBookingDate"></param>
        /// <param name="endBookingDate"></param>
        /// <returns></returns>
        private bool EditJourneyToDB(int id, string selectedVehicle, string startOdometer,
            string endOdometer, string startJourneyDate, string endJourneyDate, string journeyFrom, string journeyTo)
        {
            string editJourneySQL = "UPDATE `nmt_fleet_manager`.`journeys`" + " SET  selected_vehicle = '" + selectedVehicle + "', start_odometer = '" + startOdometer + "', " +
                "end_odometer = '" + endOdometer + "', journey_start_at = STR_TO_DATE( '" + startJourneyDate + "', '%d/%m/%Y %H:%i:%s'), journey_end_at = STR_TO_DATE( '" + endJourneyDate + "', '%d/%m/%Y %H:%i:%s')," +
                "journey_from = '" + journeyFrom + "', journey_to = '" + journeyTo + "'" +
                "WHERE id = '" + id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(editJourneySQL, connection))
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
