using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastucture.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataContext _context;
        private readonly IOrderRepository _orderRepository;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PaymentRepository(DataContext context, IOrderRepository orderRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        #region Get Data

        public async Task<PaginatedListDTO<Payment>> GetPaymentsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var result = new PaginatedListDTO<Payment>()
            {
                Data = new List<Payment>(),
                RecordCount = 0
            };

            try
            {
                var recordCount = (
                from payment in _context.Payments
                join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
                //join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
                //join customer in _context.Customers on order.CustomerId equals customer.Id
                where payment.IsActive == true && payment.IsDeleted == false &&
                    ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
                     (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber != null ? payment.ReferenceNumber.Trim().Contains(search) : false) ||
                     (string.IsNullOrEmpty(search) ? true : paymentDetail.OrderNumber.ToString().Contains(search)) ||
                     (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search))) 
                     //||(string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
                select payment)
                .Distinct()
                .Count();

                if (recordCount > 0)
                {
                    var payments = await (
                    from payment in _context.Payments
                    join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
                    //join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
                    //join customer in _context.Customers on order.CustomerId equals customer.Id
                    where payment.IsActive == true && payment.IsDeleted == false &&
                        ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
                         (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber != null ? payment.ReferenceNumber.Trim().Contains(search) : false) ||
                         (string.IsNullOrEmpty(search) ? true : paymentDetail.OrderNumber.ToString().Contains(search)) ||
                         (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search))) 
                         //||(string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
                    select payment)
                    .Distinct()
                    .OrderByDescending(e => e.Id)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

                    foreach (var payment in payments)
                    {
                        payment.PaymentDetails = await _context.PaymentDetails.Where(e => e.PaymentId == payment.Id).ToListAsync();
                    }

                    if (payments.Any())
                    {
                        result.Data = payments;
                        result.RecordCount = recordCount;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            


            //// First check if search value is integer

            //if (int.TryParse(search, out int intSearch))
            //{
            //    var recordCount = (
            //    from payment in _context.Payments
            //    join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
            //    join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
            //    join customer in _context.Customers on order.CustomerId equals customer.Id
            //    where payment.IsActive == true && payment.IsDeleted == false &&
            //        ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber.Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : order.OrderNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
            //    select payment)
            //    .Distinct()
            //    .Count();

            //    var payments = await (
            //        from payment in _context.Payments
            //        join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
            //        join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
            //        join customer in _context.Customers on order.CustomerId equals customer.Id
            //        where payment.IsActive == true && payment.IsDeleted == false &&
            //            ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber.Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : order.OrderNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
            //        select payment)
            //        .Distinct()
            //        .OrderByDescending(e => e.Id)
            //        .Skip(pageSize * pageIndex)
            //        .Take(pageSize)
            //        .ToListAsync();

            //    foreach (var payment in payments)
            //    {
            //        payment.PaymentDetails = await _context.PaymentDetails.Where(e => e.PaymentId == payment.Id).ToListAsync();
            //    }

            //    var result = new PaginatedListDTO<Payment>()
            //    {
            //        Data = new List<Payment>(),
            //        RecordCount = 0
            //    };

            //    if (payments.Any())
            //    {
            //        result.Data = payments;
            //        result.RecordCount = recordCount;
            //    }

            //    return result;
            //}
            //else
            //{
            //    var recordCount = (
            //    from payment in _context.Payments
            //    join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
            //    join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
            //    join customer in _context.Customers on order.CustomerId equals customer.Id
            //    where payment.IsActive == true && payment.IsDeleted == false &&
            //        ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber.Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : order.OrderNumber.ToString().Contains(search)) ||
            //         (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
            //    select payment)
            //    .Distinct()
            //    .Count();

            //    var payments = await (
            //        from payment in _context.Payments
            //        join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
            //        join order in _context.Orders on paymentDetail.InvoiceNumber equals order.InvoiceNumber
            //        join customer in _context.Customers on order.CustomerId equals customer.Id
            //        where payment.IsActive == true && payment.IsDeleted == false &&
            //            ((string.IsNullOrEmpty(search) ? true : payment.AccountNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : payment.ReferenceNumber.Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : paymentDetail.InvoiceNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : order.OrderNumber.ToString().Contains(search)) ||
            //             (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Contains(search)))
            //        select payment)
            //        .Distinct()
            //        .OrderByDescending(e => e.Id)
            //        .Skip(pageSize * pageIndex)
            //        .Take(pageSize)
            //        .ToListAsync();

            //    foreach (var payment in payments)
            //    {
            //        payment.PaymentDetails = await _context.PaymentDetails.Where(e => e.PaymentId == payment.Id).ToListAsync();
            //    }

            //    var result = new PaginatedListDTO<Payment>()
            //    {
            //        Data = new List<Payment>(),
            //        RecordCount = 0
            //    };

            //    if (payments.Any())
            //    {
            //        result.Data = payments;
            //        result.RecordCount = recordCount;
            //    }

            //    return result;
            //}
        }

        public async Task<PaginatedListDTO<Payment>> GetPaymentsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            // US Settings
            DateTime fDate = fromDate; // new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
            DateTime tDate = toDate.AddDays(1); // new DateTime(toDate.Year, toDate.Month, toDate.Day + 1, toDate.Hour, toDate.Minute, toDate.Second);

            //// PH Settings
            //DateTime fDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day - 1, 16, 00, 00);
            //DateTime tDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 16, 00, 00);

            var recordCount = (
                from payment in _context.Payments
                where payment.IsActive == true && payment.IsDeleted == false &&
                    (payment.CreatedDate.Date >= fDate.Date && payment.CreatedDate.Date < tDate.Date)
                select payment)
                .Distinct()
                .Count();

            var payments = await (
                from payment in _context.Payments
                where payment.IsActive == true && payment.IsDeleted == false &&
                    (payment.CreatedDate.Date >= fDate.Date && payment.CreatedDate.Date < tDate.Date)
                select payment)
                .Distinct()
                .OrderByDescending(e => e.PaymentDate)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var payment in payments)
            {
                payment.PaymentDetails = await _context.PaymentDetails.Where(e => e.PaymentId == payment.Id).ToListAsync();
            }

            var result = new PaginatedListDTO<Payment>()
            {
                Data = new List<Payment>(),
                RecordCount = 0
            };

            if (payments.Any())
            {
                result.Data = payments;
                result.RecordCount = recordCount;
            }

            return result;
        }

        public async Task<Payment?> GetPayment(int paymentId)
        {
            var result = await _context.Payments.FindAsync(paymentId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<PaymentHistoryDTO>> GetPaymentHistoryByOrderNumber(int orderNumber)
        {
            var result = new List<PaymentHistoryDTO>();
            var payments = await(
                from payment in _context.Payments
                join paymentDetail in _context.PaymentDetails on payment.Id equals paymentDetail.PaymentId
                where payment.IsActive == true && payment.IsDeleted == false && paymentDetail.OrderNumber == orderNumber
                select payment)
                .Distinct()
                .OrderBy(e => e.PaymentDate)
                .ToListAsync();

            foreach (var payment in payments)
            {
                payment.PaymentDetails = await _context.PaymentDetails.Where(e => e.PaymentId == payment.Id && e.OrderNumber == orderNumber).ToListAsync();

                string paymentType = payment.PaymentType != null ? payment.PaymentType : "";

                foreach (var paymentDetail in payment.PaymentDetails)
                {
                    decimal paymentAmount = 0;
                    if (paymentType == "Credit Memo")
                    {
                        paymentAmount = (paymentDetail.CustomerCreditAmountUsed != null) ? paymentDetail.CustomerCreditAmountUsed.Value : 0;
                    }
                    else paymentAmount = paymentDetail.PaymentAmount;

                    result.Add(
                    new PaymentHistoryDTO()
                    {
                        Id = paymentDetail.PaymentId,
                        CustomerCreditAmountUsed = paymentDetail.CustomerCreditAmountUsed != null ? paymentDetail.CustomerCreditAmountUsed.Value : 0,
                        PaymentAmount = payment.PaymentAmount != null ? payment.PaymentAmount.Value : 0,
                        PaymentBalance = payment.PaymentBalance != null ? payment.PaymentBalance.Value : 0,
                        PaymentDate = payment.PaymentDate != null ? payment.PaymentDate.Value : new DateTime(),
                        PaymentType = payment.PaymentType != null ? payment.PaymentType : "",
                        ReferenceNumber = payment.ReferenceNumber != null ? payment.ReferenceNumber : "",
                        TotalAmountDue = payment.TotalAmountDue != null ? payment.TotalAmountDue.Value : 0,
                        InvoiceAmount = paymentDetail.InvoiceAmount,
                        InvoiceBalance = paymentDetail.InvoiceBalance,
                        InvoicePaymentAmount = paymentAmount,
                        LinkedInvoiceNumber = paymentDetail.LinkedInvoiceNumber
                    });
                }
            }

            return result;
        }

        public async Task<DailyPaymentSummaryDTO> GetDailyPaymentSummary(DateTime currentDate)
        {
            DailyPaymentSummaryDTO? result = new DailyPaymentSummaryDTO();

            try
            {
                // US Settings
                DateTime frDate = currentDate; //new DateTime(dt.Year, dt.Month, dt.Day, 8, 00, 00);
                DateTime toDate = currentDate.AddDays(1); //new DateTime(dt.Year, dt.Month, dt.Day + 1, 8, 00, 00);

                //// PH Settings
                //DateTime frDate = new DateTime(dt.Year, dt.Month, dt.Day - 1, 16, 00, 00);
                //DateTime toDate = new DateTime(dt.Year, dt.Month, dt.Day, 16, 00, 00);


                #region California
                var payments = await (
                    from payment in _context.Payments
                    join customer in _context.Customers on payment.CustomerId equals customer.Id
                    where (payment.CreatedDate >= frDate && payment.CreatedDate < toDate) && payment.IsActive && !payment.IsDeleted && customer.State.Trim() == "CA"
                    select payment)
                    .Distinct()
                    .ToListAsync();
                
                if (payments != null && payments.Count > 0)
                {
                    result = new DailyPaymentSummaryDTO();
                    result.CASummary.CashAmount = payments.Where(e => e.PaymentType == "Cash").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CheckAmount = payments.Where(e => e.PaymentType == "Check").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CreditCardAmount = payments.Where(e => e.PaymentType == "Credit Card").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.ACHAmount = payments.Where(e => e.PaymentType == "ACH").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.WriteOff = payments.Where(e => e.PaymentType == "Write Off").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CreditMemoAmount = payments.Where(e => e.PaymentType == "Credit Memo").Sum(s => s.CustomerCreditAmountUsed).Value;
                    result.CASummary.TotalAmount = result.CASummary.CashAmount + result.CASummary.CheckAmount + result.CASummary.CreditCardAmount + result.CASummary.ACHAmount + result.CASummary.WriteOff;
                }

                #endregion

                #region Nevada
                payments = await (
                    from payment in _context.Payments
                    join customer in _context.Customers on payment.CustomerId equals customer.Id
                    where (payment.CreatedDate >= frDate && payment.CreatedDate < toDate) && payment.IsActive && !payment.IsDeleted && customer.State.Trim() == "NV"
                    select payment)
                    .Distinct()
                    .ToListAsync();

                if (payments != null && payments.Count > 0)
                {
                    if (result == null) result = new DailyPaymentSummaryDTO();
                    result.NVSummary.CashAmount = payments.Where(e => e.PaymentType == "Cash").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CheckAmount = payments.Where(e => e.PaymentType == "Check").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CreditCardAmount = payments.Where(e => e.PaymentType == "Credit Card").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.ACHAmount = payments.Where(e => e.PaymentType == "ACH").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.WriteOff = payments.Where(e => e.PaymentType == "Write Off").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CreditMemoAmount = payments.Where(e => e.PaymentType == "Credit Memo").Sum(s => s.CustomerCreditAmountUsed).Value;
                    result.NVSummary.TotalAmount = result.NVSummary.CashAmount + result.NVSummary.CheckAmount + result.NVSummary.CreditCardAmount + result.NVSummary.ACHAmount + result.NVSummary.WriteOff;
                }
                #endregion

            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        public async Task<DailyPaymentSummaryDTO> GetPaymentSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            DailyPaymentSummaryDTO? result = new DailyPaymentSummaryDTO();

            try
            {
                // US Settings
                DateTime fDate = fromDate; // new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
                DateTime tDate = toDate.AddDays(1); // new DateTime(toDate.Year, toDate.Month, toDate.Day + 1, toDate.Hour, toDate.Minute, toDate.Second);

                //// PH Settings
                //DateTime fDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day - 1, 16, 00, 00);
                //DateTime tDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 16, 00, 00);

                #region California
                var payments = await(
                    from payment in _context.Payments
                    join customer in _context.Customers on payment.CustomerId equals customer.Id
                    where (payment.CreatedDate >= fDate && payment.CreatedDate < tDate) && payment.IsActive && !payment.IsDeleted && customer.State.Trim() == "CA"
                    select payment)
                    .Distinct()
                    .ToListAsync();

                if (payments != null && payments.Count > 0)
                {
                    result = new DailyPaymentSummaryDTO();
                    result.CASummary.CashAmount = payments.Where(e => e.PaymentType == "Cash").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CheckAmount = payments.Where(e => e.PaymentType == "Check").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CreditCardAmount = payments.Where(e => e.PaymentType == "Credit Card").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.ACHAmount = payments.Where(e => e.PaymentType == "ACH").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.WriteOff = payments.Where(e => e.PaymentType == "Write Off").Sum(s => s.PaymentAmount).Value;
                    result.CASummary.CreditMemoAmount = payments.Where(e => e.PaymentType == "Credit Memo").Sum(s => s.CustomerCreditAmountUsed).Value;
                    result.CASummary.TotalAmount = result.CASummary.CashAmount + result.CASummary.CheckAmount + result.CASummary.CreditCardAmount + result.CASummary.ACHAmount + result.CASummary.WriteOff;
                }

                #endregion

                #region Nevada
                payments = await(
                    from payment in _context.Payments
                    join customer in _context.Customers on payment.CustomerId equals customer.Id
                    where (payment.CreatedDate >= fDate && payment.CreatedDate < tDate) && payment.IsActive && !payment.IsDeleted && customer.State.Trim() == "NV"
                    select payment)
                    .Distinct()
                    .ToListAsync();

                if (payments != null && payments.Count > 0)
                {
                    if (result == null) result = new DailyPaymentSummaryDTO();
                    result.NVSummary.CashAmount = payments.Where(e => e.PaymentType == "Cash").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CheckAmount = payments.Where(e => e.PaymentType == "Check").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CreditCardAmount = payments.Where(e => e.PaymentType == "Credit Card").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.ACHAmount = payments.Where(e => e.PaymentType == "ACH").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.WriteOff = payments.Where(e => e.PaymentType == "Write Off").Sum(s => s.PaymentAmount).Value;
                    result.NVSummary.CreditMemoAmount = payments.Where(e => e.PaymentType == "Credit Memo").Sum(s => s.CustomerCreditAmountUsed).Value;
                    result.NVSummary.TotalAmount = result.NVSummary.CashAmount + result.NVSummary.CheckAmount + result.NVSummary.CreditCardAmount + result.NVSummary.ACHAmount + result.NVSummary.WriteOff;
                }
                #endregion

            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }
        
        #endregion

        #region Save Data

        public async Task<Payment> Create(Payment payment)
        {
            try
            {
                decimal overPayment = 0;
                // Check first if there is an overpayment
                if (payment.PaymentBalance < 0)
                {
                    overPayment = Math.Abs(payment.PaymentBalance.Value);
                    payment.PaymentBalance = 0;
                }

                _context.Payments.Add(payment);
                await _context.SaveEntitiesAsync();

                foreach (var paymentDetail in payment.PaymentDetails)
                {
                    paymentDetail.PaymentId = payment.Id;
                }

                await _context.PaymentDetails.AddRangeAsync(payment.PaymentDetails);
                await _context.SaveEntitiesAsync();

                //Update Order With Payment Details
                foreach (var paymentDetail in payment.PaymentDetails)
                {
                    var order = await _context.Orders.AsNoTracking().Where(e => e.InvoiceNumber == paymentDetail.InvoiceNumber).FirstOrDefaultAsync();

                    if (order != null)
                    {
                        order.OrderStatusId = 2;
                        order.OrderStatusName = "Posted";

                        order.AmountPaid += paymentDetail.PaymentAmount;
                        order.Balance = paymentDetail.InvoiceBalance;
                        order.Payment = (paymentDetail.LinkedInvoiceNumber != null) ? (order.Payment != null) ? order.Payment + paymentDetail.CustomerCreditAmountUsed : paymentDetail.CustomerCreditAmountUsed : order.Payment;
                        order.PaymentReference = order.Balance > 0 ? "Partial" : "Full";

                        if (paymentDetail.LinkedInvoiceNumber != null)
                        {
                            order.LinkedInvoiceNumber = (order.LinkedInvoiceNumber != null) ? order.LinkedInvoiceNumber + "," + paymentDetail.LinkedInvoiceNumber : paymentDetail.LinkedInvoiceNumber;
                        }
                        
                        _context.Orders.Update(order);
                    }
                }

                await _context.SaveEntitiesAsync();

                //Update Credit Memo (Order) if used for Payment
                if (payment.CreditMemos != null && payment.CreditMemos.Count > 0)
                {
                    foreach (var credMemo in payment.CreditMemos)
                    {
                        var cm = await _context.Orders.AsNoTracking().Where(e => e.Id == credMemo.Id).FirstOrDefaultAsync();
                        if (cm != null)
                        {
                            cm.AmountPaid = credMemo.AmountPaid;
                            cm.Balance = credMemo.Balance;
                            cm.Payment = credMemo.Payment;
                            cm.LinkedInvoiceNumber = credMemo.LinkedInvoiceNumber;

                            cm.ModifiedBy = payment.CreatedBy;
                            cm.ModifiedDate = payment.CreatedDate;

                            await _orderRepository.UpdateCreditMemo(cm);
                        }
                    }
                }

                //Update Customer Credit
                if (payment.CustomerCreditId != null)
                {
                    var customerCredit = await _context.CustomerCredits.Where(e => e.Id == payment.CustomerCreditId).FirstOrDefaultAsync();
                    if (customerCredit != null)
                    {
                        var newCustomerCredit = new CustomerCredit()
                        {
                            AmountPosted = payment.CustomerCreditAmountUsed != null ? payment.CustomerCreditAmountUsed.Value : 0,
                            AmountPostedDate = payment.CreatedDate,
                            CreatedBy = payment.CreatedBy,
                            CreatedDate = payment.CreatedDate,
                            CreditReason = "Payment",
                            CreditType = 1, //Payment
                            CurrentBalance = customerCredit.CurrentBalance - (payment.CustomerCreditAmountUsed != null ? payment.CustomerCreditAmountUsed.Value : 0),
                            CustomerId = payment.CustomerId,
                            IsActive = payment.IsActive,
                            IsDeleted = payment.IsDeleted,
                            PreviousBalance = customerCredit.CurrentBalance,
                            PreviousRecordId = customerCredit.Id,
                            ReferenceId = payment.Id
                        };

                        customerCredit.IsActive = false;
                        _context.CustomerCredits.Update(customerCredit);
                        await _context.CustomerCredits.AddAsync(newCustomerCredit);
                        await _context.SaveEntitiesAsync();
                    }
                }

                if (overPayment > 0)
                {
                    var customerCredit = await _context.CustomerCredits.Where(e => e.CustomerId == payment.CustomerId && e.IsActive == true).FirstOrDefaultAsync();
                    var newCustomerCredit = new CustomerCredit()
                    {
                        AmountPosted = overPayment,
                        AmountPostedDate = payment.CreatedDate,
                        CreatedBy = payment.CreatedBy,
                        CreatedDate = payment.CreatedDate,
                        CreditReason = "Over Payment",
                        CreditType = 1, //Payment
                        CurrentBalance = customerCredit != null ? customerCredit.CurrentBalance + overPayment : overPayment,
                        CustomerId = payment.CustomerId,
                        IsActive = payment.IsActive,
                        IsDeleted = payment.IsDeleted,
                        PreviousBalance = customerCredit != null ? customerCredit.CurrentBalance : 0,
                        PreviousRecordId = customerCredit != null ? customerCredit.Id : 0,
                        ReferenceId = payment.Id
                    };

                    if (customerCredit != null)
                    {
                        customerCredit.IsActive = false;
                        _context.CustomerCredits.Update(customerCredit);
                    }

                    await _context.CustomerCredits.AddAsync(newCustomerCredit);
                    await _context.SaveEntitiesAsync();
                }

                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Payment> CreateRefund(Payment payment)
        {
            try
            {
                _context.Payments.Add(payment);
                await _context.SaveEntitiesAsync();

                foreach (var paymentDetail in payment.PaymentDetails)
                {
                    paymentDetail.PaymentId = payment.Id;
                }

                await _context.PaymentDetails.AddRangeAsync(payment.PaymentDetails);
                await _context.SaveEntitiesAsync();

                //Update Order With Payment Details
                foreach (var paymentDetail in payment.PaymentDetails)
                {
                    var order = await _context.Orders.AsNoTracking().Where(e => e.InvoiceNumber == paymentDetail.InvoiceNumber).FirstOrDefaultAsync();

                    if (order != null)
                    {
                        order.OrderStatusId = 6;
                        order.OrderStatusName = "Refund";

                        order.AmountPaid += paymentDetail.PaymentAmount;
                        order.Balance = 0;
                        order.Payment = (order.Payment != null) ? order.Payment + paymentDetail.PaymentAmount : order.Payment;
                        order.PaymentReference = "Full";

                        if (paymentDetail.LinkedInvoiceNumber != null)
                        {
                            order.LinkedInvoiceNumber = (order.LinkedInvoiceNumber != null) ? order.LinkedInvoiceNumber + "," + paymentDetail.LinkedInvoiceNumber : paymentDetail.LinkedInvoiceNumber;
                        }

                        _context.Orders.Update(order);
                    }
                }

                await _context.SaveEntitiesAsync();
                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> Update(Payment payment)
        {
            _context.Payments.Update(payment);
            return await _context.SaveEntitiesAsync();
        }

        public async Task<List<Payment>> Delete(List<int> paymentIds)
        {
            var payments = _context.Payments.Where(a => paymentIds.Contains(a.Id)).ToList();
            _context.Payments.RemoveRange(payments);
            await _context.SaveEntitiesAsync();
            return await _context.Payments.ToListAsync();
        }

        public async Task<List<Payment>> SoftDelete(List<int> paymentIds)
        {
            var payments = _context.Payments.Where(a => paymentIds.Contains(a.Id)).ToList();
            payments.ForEach(a => { a.IsDeleted = true; });

            _context.Payments.UpdateRange(payments);
            await _context.SaveEntitiesAsync();
            return await _context.Payments.ToListAsync();
        }
        #endregion
    }
}
