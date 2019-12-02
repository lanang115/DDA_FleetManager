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
    /// Interaction logic for EditService.xaml
    /// </summary>
    public partial class EditService : Window
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

        /// <summary>
        /// this is a constructor for this window
        /// </summary>
        public EditService()
        {
            InitializeComponent();
            InitializeDb();
            OpenConnection();
            CloseConnection();
        }

        public EditService(int id, string selectedVehicle, int serviceOdometer, DateTime serviceDate)
        {
            InitializeComponent();
            InitializeDb();
            this.id = id;
            TextBoxSelectedVehicle.Text = selectedVehicle;
            TextBoxLastOdometerForService.Text = serviceOdometer.ToString();
            DatePickerForLastServiceDate.Text = serviceDate.ToString();
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
        /// this is a click event for edit service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveChangesService_Clicked(object sender, RoutedEventArgs e)
        {
            string selectedVehicle = TextBoxSelectedVehicle.Text;
            string serviceOdometer = TextBoxLastOdometerForService.Text;
            string serviceDate = DatePickerForLastServiceDate.Text;

            EditServiceToDB(id, selectedVehicle, serviceOdometer, serviceDate);
            this.DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// this is a click event for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelChangesService_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a method for editService
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedVehicle"></param>
        /// <param name="startOdometer"></param>
        /// <param name="endOdometer"></param>
        /// <param name="startJourneyDate"></param>
        /// <param name="endJourneyDate"></param>
        /// <param name="journeyFrom"></param>
        /// <param name="journeyTo"></param>
        /// <returns></returns>
        private bool EditServiceToDB(int id, string selectedVehicle, string serviceOdometer,
           string serviceDate)
        {
            string editServiceSQL = "UPDATE `nmt_fleet_manager`.`services`" + " SET  selected_vehicle = '" + selectedVehicle + "', service_odometer = '" + serviceOdometer + "', " +
                "service_at = STR_TO_DATE( '" + serviceDate + "', '%d/%m/%Y %H:%i:%s')"+ "WHERE id = '" + id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(editServiceSQL, connection))
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
