using System;
using System.Collections.Generic;
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
    /// Interaction logic for BookingForm.xaml
    /// </summary>
    public partial class BookingForm : Window
    {
        private int newId;

        public BookingForm()
        {
            InitializeComponent();
        }

        public BookingForm(string carManufacture, string carModel, int vehicleOdometer)
        {
            InitializeComponent();
            // set value on booking start odometer textbox
            BookingStartOdometerTextBox.Text = vehicleOdometer.ToString();
            // set text on selected vehicle textbox
            SelectedVehicleTextBox.Text = carManufacture + " " + carModel.ToString();
            // items for comboBox
            ComboBoxRentalType.Items.Add(BookingType.Day);
            ComboBoxRentalType.Items.Add(BookingType.Km);
        }

        public BookingForm(string carManufacture, string carModel, int vehicleOdometer, int newId) : this(carManufacture, carModel, vehicleOdometer)
        {
            this.newId = newId;
        }

        private void cancelBookButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
