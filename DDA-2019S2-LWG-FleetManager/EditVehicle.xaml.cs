using Bogus.DataSets;
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
   
    public partial class EditVehicle : Window
    {
        private string server;
        private string database;
        private string db_user;
        private string password;
        private int port;
        public Vehicle vehicles;
        /// <summary>
        /// DSN String (Data Source Name)
        /// </summary>
        private string dsnString;

        /// <summary>
        /// Property: Database connection
        /// </summary>
        private MySqlConnection connection;
        public CarList car;
        public int id;

        public EditVehicle()
        {
            InitializeComponent();
            HideErrors();
            InitializeDb();
            OpenConnection();
            CloseConnection();
        }

        public EditVehicle(int id, string regisId, string manufacture, string model, int year, double fuelCapacity, int odometer)
        {
            InitializeComponent();
            HideErrors();
            InitializeDb();
            this.id = id;
            TextBoxRegisId.Text = regisId;
            TextBoxManufacture.Text = manufacture;
            TextBoxModel.Text = model;
            TextBoxYear.Text = year.ToString();
            TextBoxFuelCapacity.Text = fuelCapacity.ToString();
            TextBoxOdometer.Text = odometer.ToString();
        }

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

        private void HideErrors()
        {
            registrationIdError.Visibility = Visibility.Hidden;
            carManufactureError.Visibility = Visibility.Hidden;
            carModelError.Visibility = Visibility.Hidden;
            carYearError.Visibility = Visibility.Hidden;
            carOdometerError.Visibility = Visibility.Hidden;
            fuelCapacityError.Visibility = Visibility.Hidden;
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

        private void buttonCancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonSaveVehicle_Clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
            string regisId = TextBoxRegisId.Text;
            string carManufacture = TextBoxManufacture.Text;
            string carModel = TextBoxModel.Text;
            string carYear = TextBoxYear.Text;
            string fuelCapacity = TextBoxFuelCapacity.Text;
            string carOdometer = TextBoxOdometer.Text;

            EditVehicleToDB(id, regisId, carManufacture, carModel,
                   carYear, fuelCapacity, carOdometer);
            this.DialogResult = true;
            this.Close();
        }

        private bool EditVehicleToDB(int id, string regisId, string carManufacture, string carModel,
            string carYear, string fuelCapacity, string carOdometer)
        {
            string editVehicleSQL = "UPDATE `nmt_fleet_manager`.`vehicles`" + " SET  registration_id = '" + regisId + "', car_manufacture = '" + carManufacture + "', " +
                "car_model = '" + carModel + "', car_year = '" + carYear + "', tank_capacity = '" + fuelCapacity + "', vehicle_odometer = '" + carOdometer + "'" +
                "WHERE id = '" + id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(editVehicleSQL, connection))
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
