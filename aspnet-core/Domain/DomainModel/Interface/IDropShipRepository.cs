using Domain.DomainModel.Entity.DropShip;
using Domain.DomainModel.Entity.DTO.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Interface
{
    public interface IDropShipRepository : IRepository<PartInquiry>
    {
        Task<CheckInventoryResponse> CheckInventory(CheckInventoryRequest request);
        Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request);
        Task<OrderStatusResponse> OrderStatus(OrderStatusRequest request);
        void GenerateBellFlowerDailyReport(int triggerHour, int triggerMinute);
    }
}
