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
    /// Interaction logic for EditBooking.xaml
    /// </summary>
    public partial class EditBooking : Window
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
        private int id;
        /// <summary>
        /// Property: Database connection
        /// </summary>
        private MySqlConnection connection;
        public EditBooking()
        {
            InitializeComponent();
            HideErrors();
            InitializeDb();
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
        /// this is a constructor to set the textbox
        /// when the user click on edit booking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerName"></param>
        /// <param name="startOdometer"></param>
        /// <param name="startRentDate"></param>
        /// <param name="endRentDate"></param>
        /// <param name="rentalType"></param>
        /// <param name="selectedVehicle"></param>
        public EditBooking(int id,string customerName, int startOdometer, DateTime startRentDate, DateTime endRentDate, BookingType rentalType, string selectedVehicle)
        {
            InitializeComponent();
            HideErrors();
            InitializeDb();
            this.id = id;
            CustomerNameTextBox.Text = customerName;
            BookingStartOdometerTextBox.Text = startOdometer.ToString();
            BookingStartDatePicker.Text = startRentDate.ToString();
            BookingEndDatePicker.Text = endRentDate.ToString();
            SelectedVehicleTextBox.Text = selectedVehicle;
            ComboBoxRentalType.Items.Add(BookingType.Day);
            ComboBoxRentalType.Items.Add(BookingType.Km);
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
        /// this is a method to hide Error label
        /// </summary>
        private void HideErrors()
        {
            LabelRentDateError.Visibility = Visibility.Hidden;
            LabelEndDateError.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// this is a click event to close editBooking windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBookingCancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// this is a click event to save editBooking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveEditBooking_Clicked(object sender, RoutedEventArgs e)
        {
            HideErrors();
            string customerName = CustomerNameTextBox.Text;
            string selectedVehicle = SelectedVehicleTextBox.Text;
            string bookingType = ComboBoxRentalType.Text;
            string startOdometer = BookingStartOdometerTextBox.Text;
            string startBookingDate = BookingStartDatePicker.Text;
            string endBookingDate = BookingEndDatePicker.Text;
            bool validStartDate = ValidateStartDate(startBookingDate);
            bool validEndDate = ValidateEndDate(endBookingDate);

            if (!validStartDate)
            {
                // TODO: Display error to the user
                LabelRentDateError.Content = "cannot be null";
                LabelRentDateError.Visibility = Visibility.Visible;
            }
            else if (!validEndDate)
            {
                // TODO: Display error to the user
                LabelEndDateError.Content = "cannot be null";
                LabelEndDateError.Visibility = Visibility.Visible;
            }
            if (validStartDate && validEndDate)
            {
                EditBookingToDB(id,customerName, selectedVehicle, bookingType,
                    startOdometer, startBookingDate, endBookingDate);
                this.DialogResult = true;
                this.Close();
            }

        }

        /// <summary>
        /// this is a bool to check if its null or not
        /// </summary>
        /// <param name="regisId"></param>
        /// <returns></returns>
        private bool ValidateStartDate(string startBookingDate)
        {
            if (startBookingDate == null)
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
        private bool ValidateEndDate(string endBookingDate)
        {
            if (endBookingDate == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// this is a method to edit booking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="regisId"></param>
        /// <param name="carManufacture"></param>
        /// <param name="carModel"></param>
        /// <param name="carYear"></param>
        /// <param name="fuelCapacity"></param>
        /// <param name="carOdometer"></param>
        /// <returns></returns>
        private bool EditBookingToDB(int id, string customerName, string selectedVehicle, string bookingType,
            string startOdometer, string startBookingDate, string endBookingDate)
        {
            string editBookingSQL = "UPDATE `nmt_fleet_manager`.`bookings`" + " SET  customer_name = '" + customerName + "', selected_vehicle = '" + selectedVehicle + "', " +
                "booking_type = '" + bookingType + "', start_odometer = '" + startOdometer + "', start_at = STR_TO_DATE( '" + startBookingDate + "', '%d/%m/%Y %H:%i:%s'), end_at = STR_TO_DATE( '" + endBookingDate + "', '%d/%m/%Y %H:%i:%s')" +
                "WHERE id = '" + id + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(editBookingSQL, connection))
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
