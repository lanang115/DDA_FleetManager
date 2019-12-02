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
    /// Interaction logic for AddFuelPurchase.xaml
    /// </summary>
    public partial class AddFuelPurchase : Window
    {
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

        /// <summary>
        /// this is a constructor for this window
        /// </summary>
        public AddFuelPurchase()
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

        public AddFuelPurchase(string selectedVehicle, int vehicleid)
        {
            InitializeComponent();
            InitializeDb();
            OpenConnection();
            CloseConnection();
            TextBoxSelectedVehicle.Text = selectedVehicle;
            this.vehicleid = vehicleid;
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
        /// this is a click event for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancelFuel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a click event to add fuel purchase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddFuel_Clicked(object sender, RoutedEventArgs e)
        {
            decimal fuelQuantity = decimal.Parse(TextBoxFuelQuantity.Text);
            decimal fuelPrice = decimal.Parse(TextBoxFuelPrice.Text);

            FuelPurchase fuelPurchase = new FuelPurchase(fuelQuantity, fuelPrice);
            fuelPurchase.VId = vehicleid;
            string selectedVehicle = TextBoxSelectedVehicle.Text;
            string FuelQuantity = fuelPurchase.FuelQuantity.ToString();
            string FuelPrice = fuelPurchase.FuelPrice.ToString();
            string totalCost = fuelPurchase.TotalCost.ToString();

            AddFuelPurchaseToDB(vehicleid,selectedVehicle,FuelQuantity,
                FuelPrice,totalCost);
            this.DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// this is a method to addFuel to database
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="selectedVehicle"></param>
        /// <param name="FuelQuantity"></param>
        /// <param name="FuelPrice"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        private bool AddFuelPurchaseToDB(int vehicleId, string selectedVehicle, string FuelQuantity, string FuelPrice, string totalCost)
        {
            string addFuelSQL = "INSERT INTO `nmt_fleet_manager`.`fuel_purchases`(vehicle_id, selected_vehicle, fuel_quantity, fuel_price, total_cost)"
                + " VALUES ('" + vehicleId + "', '" + selectedVehicle + "', '" + FuelQuantity + "', '" + FuelPrice + "', '" + totalCost + "' )";

            using (MySqlCommand cmdSel = new MySqlCommand(addFuelSQL, connection))
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
