using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA_2019S2_LWG_FleetManager
{
    public class FuelPurchase
    {
        public int id { get; set; }
        public int VId { get; set; }
        public string SelectedVehicle { get; set; }
        public decimal FuelQuantity { get; set; }
        public decimal FuelPrice { get; set; }
        public decimal TotalCost { get; set; }

        public FuelPurchase(int id)
        {
            this.id = id;
        }

        public FuelPurchase(decimal fuelQuantity, decimal fuelPrice)
        {
            this.FuelQuantity = fuelQuantity;
            this.FuelPrice = fuelPrice;
            this.TotalCost = fuelQuantity * fuelPrice;
        }
    }
}
