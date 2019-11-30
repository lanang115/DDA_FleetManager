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
    /// Interaction logic for AddVehicle.xaml
    /// </summary>
    public partial class AddVehicle : Window
    {
        public CarList carlist;
        /// <summary>
        /// Interaction logic for LocationEdit.xaml
        /// </summary>
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

        public AddVehicle()
        {
            InitializeComponent();
            InitializeDb();
            HideErrors();
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

        private void buttonCancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Hide error messages
        /// </summary>
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
        /// <summary>
        /// this is a click event to add vehicle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveVehicle_Clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
            string regisId = TextBoxRegisId.Text;
            string carManufacture = TextBoxManufacture.Text;
            string carModel = TextBoxModel.Text;
            string carYear = TextBoxYear.Text;
            string fuelCapacity = TextBoxFuelCapacity.Text;
            string carOdometer = TextBoxOdometer.Text;
            bool validRegisId = ValidateRegisId(regisId);
            bool validManufacture = ValidateCarManufacture(carManufacture);
            bool validModel = ValidateCarModel(carModel);
            bool validYear = ValidateCarYear(carYear);
            bool validFuelCapacity = ValidateFuelCapacity(fuelCapacity);
            bool validCarOdometer = ValidateCarOdometer(carOdometer);

            if (!validRegisId)
            {
                // TODO: Display error to the user
                registrationIdError.Text = "cannot be null";
                registrationIdError.Visibility = Visibility.Visible;
            }
            else if (!validManufacture)
            {
                // TODO: Display error to the user
                carManufactureError.Text = "cannot be null";
                carManufactureError.Visibility = Visibility.Visible;
            }
            else if (!validModel)
            {
                // TODO: Display error to the user
                carModelError.Text = "cannot be null";
                carModelError.Visibility = Visibility.Visible;
            }
            else if (!validYear)
            {
                // TODO: Display error to the user
                carYearError.Text = "cannot be null";
                carYearError.Visibility = Visibility.Visible;
            }
            else if (!validFuelCapacity)
            {
                // TODO: Display error to the user
                fuelCapacityError.Text = "cannot be null";
                fuelCapacityError.Visibility = Visibility.Visible;
            }
            else if (!validCarOdometer)
            {
                // TODO: Display error to the user
                carOdometerError.Text = "cannot be null";
                carOdometerError.Visibility = Visibility.Visible;
            }
            if (validRegisId && validManufacture && validModel && validYear
                && validFuelCapacity && validCarOdometer)
            {
                AddVehicleToDB(regisId, carManufacture, carModel,
                    carYear, fuelCapacity, carOdometer);
                this.DialogResult = true;
                this.Close();
            }


        }

        private bool ValidateRegisId(string regisId)
        {
            if (regisId == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidateCarManufacture(string carManufacture)
        {
            if (carManufacture == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidateCarModel(string carModel)
        {
            if (carModel == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidateCarYear(string carYear)
        {
            if (carYear == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidateCarOdometer(string carOdometer)
        {
            if (carOdometer == null)
            {
                return false;
            }
            return true;
        }

        private bool ValidateFuelCapacity(string fuelCapacity)
        {
            if (fuelCapacity == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// this is a method to addvehicle to database
        /// </summary>
        /// <param name="regisId"></param>
        /// <param name="carManufacture"></param>
        /// <param name="carModel"></param>
        /// <param name="carYear"></param>
        /// <param name="fuelCapacity"></param>
        /// <param name="carOdometer"></param>
        /// <returns></returns>
        private bool AddVehicleToDB(string regisId, string carManufacture, string carModel,
            string carYear, string fuelCapacity, string carOdometer)
        {
            string addVehicleSQL = "INSERT INTO `nmt_fleet_manager`.`vehicles`(registration_id, car_manufacture, car_model, car_year, tank_capacity, vehicle_odometer) "
                + " VALUES ( '" + regisId + "', '" + carManufacture + "', '" + carModel + "', '" + carYear + "', '" + fuelCapacity + "', '" + carOdometer + "')";

            using (MySqlCommand cmdSel = new MySqlCommand(addVehicleSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows == 0;
            }
        }
    }
}
