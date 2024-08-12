using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class WarehouseStockDTO : BaseModel
    {
        public int WarehouseId { get; set; }
        public int WarehouseLocationId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string? Location { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }

        public int? DestinationWarehouseLocationId { get; set; }
        public string? DestinationLocation { get; set; }
    }
}
