using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA_2019S2_LWG_FleetManager
{
    public class Service
    {
        public int id {get;set;}
        public int vehicleId { get; set; }
        public int ServiceOdometer { get; set; }
        public string SelectedVehicle { get; set; }
        public DateTime ServiceDate { get; set; }
        public Service(int id)
        {
            this.id = id;
        }
    }
}
