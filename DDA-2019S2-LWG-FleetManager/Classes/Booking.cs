using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA_2019S2_LWG_FleetManager
{
    public class Booking
    {
        public int id { get; set; }
        public int Vehicleid { get; set; }
        public string CustomerName { get; set; }
        public string SelectedVehicle { get; set; }
        public BookingType RentalType { get; set; }
        public int StartOdometer { get; set; }
        public DateTime StartRentDate { get; set; }
        public DateTime EndRentDate { get; set; }
        
        public Booking(int id)
        {
            this.id = id;

        }
    }

    
}
