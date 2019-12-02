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
    /// Interaction logic for AddService.xaml
    /// </summary>
    public partial class AddService : Window
    {
        private int VehicleId;
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
        /// <summary>
        /// this is a constructor for this window
        /// </summary>
        public AddService()
        {
            InitializeComponent();
            InitializeDb();
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
        /// this is a constructor to set the textbox value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carManufacture"></param>
        /// <param name="carModel"></param>
        /// <param name="vehicleOdometer"></param>
        public AddService(int id, string carManufacture, string carModel, int vehicleOdometer)
        {
            InitializeComponent();
            InitializeDb();
            this.VehicleId = id;
            TextBoxSelectedVehicle.Text = carManufacture + " " + carModel;
            TextBoxLastOdometerForService.Text = vehicleOdometer.ToString();
        }
        /// <summary>
        /// this is a click event for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
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
        /// this is a click event for addService button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serviceNowButton_Clicked(object sender, RoutedEventArgs e)
        {
            int vehicleId = VehicleId;
            string selectedVehicle = TextBoxSelectedVehicle.Text;
            string serviceOdometer = TextBoxLastOdometerForService.Text;
            string serviceDate = DatePickerForLastServiceDate.Text;

            AddServiceToDB(vehicleId, selectedVehicle, serviceOdometer, serviceDate);
            this.DialogResult = true;
            this.Close();
        }
        
        /// <summary>
        /// this is a method for addService to database
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
        private bool AddServiceToDB(int vehicleId, string selectedVehicle, string serviceOdometer, string serviceDate)
        {
            string addServiceSQL = "INSERT INTO `nmt_fleet_manager`.`services`(vehicle_id, selected_vehicle, service_odometer, service_at)"
                + " VALUES ('" + vehicleId + "', '" + selectedVehicle + "', '" + serviceOdometer + "', STR_TO_DATE( '" + serviceDate + "', '%d/%m/%Y %H:%i:%s'))";

            using (MySqlCommand cmdSel = new MySqlCommand(addServiceSQL, connection))
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
