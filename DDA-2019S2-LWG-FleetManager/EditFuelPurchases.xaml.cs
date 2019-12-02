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
    /// Interaction logic for EditFuelPurchases.xaml
    /// </summary>
    public partial class EditFuelPurchases : Window
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
        public EditFuelPurchases()
        {
            InitializeComponent();
            InitializeDb();
            OpenConnection();
            CloseConnection();
        }
        /// <summary>
        /// this is a constructor to set textbox
        /// </summary>
        /// <param name="fuelQuantity"></param>
        /// <param name="fuelPrice"></param>
        /// <param name="selectedVehicle"></param>
        public EditFuelPurchases(int id, decimal fuelQuantity, decimal fuelPrice, string selectedVehicle)
        {
            InitializeComponent();
            InitializeDb();
            OpenConnection();
            CloseConnection();
            this.id = id;
            TextBoxFuelQuantity.Text = fuelQuantity.ToString();
            TextBoxFuelPrice.Text = fuelPrice.ToString();
            TextBoxSelectedVehicle.Text = selectedVehicle;
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
        /// this is a click event for save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveChangesButton_Clicked(object sender, RoutedEventArgs e)
        {
            decimal fuelQuantity = decimal.Parse(TextBoxFuelQuantity.Text);
            decimal fuelPrice = decimal.Parse(TextBoxFuelPrice.Text);

            FuelPurchase fuelPurchase = new FuelPurchase(fuelQuantity, fuelPrice);
            string selectedVehicle = TextBoxSelectedVehicle.Text;
            string FuelQuantity = fuelPurchase.FuelQuantity.ToString();
            string FuelPrice = fuelPurchase.FuelPrice.ToString();
            string totalCost = fuelPurchase.TotalCost.ToString();

            EditFuelPurchaseToDB(id, selectedVehicle, FuelQuantity,
                FuelPrice, totalCost);
            this.DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// this is a click event for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a method to update changes to database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedVehicle"></param>
        /// <param name="FuelQuantity"></param>
        /// <param name="FuelPrice"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        private bool EditFuelPurchaseToDB(int id, string selectedVehicle, string FuelQuantity,
            string FuelPrice, string totalCost)
        {
            string editFuelSQL = "UPDATE `nmt_fleet_manager`.`fuel_purchases`" + " SET  selected_vehicle = '" + selectedVehicle + "', fuel_quantity = '" + FuelQuantity + "', " +
                "fuel_price = '" + FuelPrice + "', total_cost = '" + totalCost + "'" + "WHERE id = '" + id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(editFuelSQL, connection))
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
