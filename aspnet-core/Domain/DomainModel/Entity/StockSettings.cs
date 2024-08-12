using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class StockSettings : BaseModel
    {
        public bool IsCaliforniaCAProduct { get; set; }
        public bool IsCaliforniaNVProduct { get; set; }
        public bool IsNevadaCAProduct { get; set; }
        public bool IsNevadaNVProduct { get; set; }
        public bool IsDropShipCAProduct { get; set; }
        public bool IsDropShipNVProduct { get; set; }
    }
}
