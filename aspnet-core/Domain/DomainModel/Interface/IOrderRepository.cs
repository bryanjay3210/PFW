using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
        #region Get Data
        Task<List<Order>> GetOrders();
        Task<Order?> GetOrderByOrderNumber(int orderNumber);
        Task<List<Order>> GetOrdersByCustomerId(int customerId);
        Task<List<Order>> GetInvoicesByCustomerId(int customerId);
        Task<List<Order>> GetInvoicesByCustomerIds(List<int> customerIds);
        Task<List<Order>> GetCreditMemoByInvoiceNumber(int invoiceNumber);
        Task<List<Order>> GetCreditMemoByCustomerId(int customerId);
        Task<List<Order>> GetCreditMemoByCustomerIds(List<int> customerIds);
        Task<List<Order>> GetQuotes();
        Task<Order?> GetOrder(int orderId);
        Task<PaginatedListDTO<Order>> GetOrdersPaginated(int searchType, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search, int? paymentTerm);
        Task<PaginatedListDTO<Order>> GetOrdersByDatePaginated(int searchType, int pageSize, int pageIndex, DateTime fromDate, DateTime toDate, string? search, int? paymentTerm);
        //Task<PaginatedListDTO<Order>> GetOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate);
        Task<PaginatedListDTO<Order>> GetCustomerOrdersPaginated(int customerId, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<Order>> GetQuotesPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<Order>> GetWebOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<Order>> GetRGAOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<Order>> GetReportOrdersListPaginated(DateTime deliveryDate, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search, string? state, int? deliveryRoute);
        Task<DailySalesSummaryDTO> GetDailySalesSummary(DateTime currentDate);
        Task<DailySalesSummaryDTO> GetDailySalesSummaryByDate(DateTime fromDate, DateTime toDate);
        Task<List<AgingBalanceReportDTO>> GetAgingBalanceReport(DateTime reportDate);
        Task<List<StatementReportDTO>> GetStatementReport(DateTime reportDate, int paymentTermId, List<int> customerIds);
        Task<List<StatementTotalReportDTO>> GetStatementTotalReport(DateTime reportDate, int paymentTermId, List<int> customerIds);
        Task<List<Order>> GetDiscountsByInvoiceNumber(int invoiceNumber, List<string> partNumbers);
        Task<List<CustomerSalesAmountDTO>> GetCustomerSales(CustomerSalesFilterDTO filter);
        Task<DeliverySummaryDTO> GetDeliverySummary(DateTime currentDate);

        #endregion

        #region Save Data
        Task<bool> VoidOrder(Order order);
        Task<bool> DeleteRGAOrder(Order order);
        Task<Order> Create(Order order);
        Task<bool> ConvertQuoteToOrder(Order order);
        Task<bool> Update(Order order);
        Task<bool> UpdateOrderStatus(Order order);
        Task<bool> UpdateOrderSummary(Order order);
        Task<bool> UpdateCreditMemo(Order order);
        Task<List<Order>> Delete(List<int> orderIds);
        Task<bool> DeleteOrderDetail(int orderDetailId);
        Task<List<Order>> SoftDelete(List<int> orderIds);
        Task<bool> CreateCreditMemo(Order order);
        Task<bool> CreateRGA(Order order);
        Task<bool> CreateDiscount(Order order);
        Task<bool> CreateOverpayment(OverpaymentParameterDTO param);
        Task<bool> UpdatePrintedInvoice(List<int> orderIds);
        Task<bool> UpdateOrderInspectedCode(Order order);
        
        //Task<bool> UpdateOrderDetailInspectedCode(OrderDetail orderdetail);
        //Task GetCustomerOrdersPaginated(int customerId, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        #endregion
    }
}
