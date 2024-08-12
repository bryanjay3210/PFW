using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class WarehouseLocationWithStockDTO
    {
        public WarehouseLocation WarehouseLocation { get; set; } = new WarehouseLocation();
        public List<Product> Stocks { get; set; } = new List<Product>();
    }
}
