using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA_2019S2_LWG_FleetManager
{
    public class Journey
    {
        public int id { get; set; }
        public int BookingID { get; set; }
        public int VehicleID { get; set; }
        public string selectedVehicle { get; set; }
        public DateTime JourneyStartAt { get; set; }
        public DateTime JourneyEndedAt { get; set; }
        public int StartOdometer { get; set; }
        public int EndOdometer { get; set; }
        public string JourneyFrom { get; set; }
        public string JourneyTo { get; set; }

        public Journey(int id)
        {
            this.id = id;
        }
    }
}
