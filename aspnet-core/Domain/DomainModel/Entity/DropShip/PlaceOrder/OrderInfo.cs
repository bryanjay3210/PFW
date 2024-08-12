using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class OrderInfo
    {
        public PlaceOrder Order { get; set; } = new PlaceOrder();
        public List<PlacePart> Parts { get; set; } = new List<PlacePart>();
    }
}
