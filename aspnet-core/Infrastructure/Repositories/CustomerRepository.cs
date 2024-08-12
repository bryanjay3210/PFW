using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CustomerRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<PaginatedListDTO<Customer>> GetCustomersPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "")
        {
            var customers = new List<Customer>();
            int recordCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                recordCount = (from c in _context.Customers select c)
                .Distinct()
                .Count();

                if (sortOrder == "ASC")
                {
                    customers = await (from c in _context.Customers select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderBy(e => "e." + sortColumn)
                    //.OrderBy(e => e.CustomerName)
                }
                else
                {
                    customers = await (from c in _context.Customers select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderByDescending(e => "e." + sortColumn)
                    //.OrderByDescending(e => e.CustomerName)
                }
            }
            else
            {
                search = search.ToLower();
                recordCount = (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Count();

                if (sortOrder == "ASC")
                {
                    customers = await (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderBy(e => "e." + sortColumn)
                    //.OrderBy(e => e.CustomerName)
                }
                else
                {
                    customers = await (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderByDescending(e => "e." + sortColumn)
                    //.OrderByDescending(e => e.CustomerName)
                }
            }

            var result = new PaginatedListDTO<Customer>()
            {
                Data = customers,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<List<CustomerDTO>> GetCustomersList()
        {
            var result = new List<CustomerDTO>();
            var customers = await _context.Customers.Where(e => e.IsActive == true && e.IsDeleted == false).OrderBy(f => f.CustomerName).ToListAsync();

            foreach (var customer in customers)
            {
                //var zone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == customer.ZipCode.Trim().ToLower()).FirstOrDefaultAsync();

                var customerDTO = new CustomerDTO()
                {
                    AccountNumber = customer.AccountNumber,
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    City = customer.City,
                    ContactName = customer.ContactName,
                    Country = customer.Country,
                    CustomerName = customer.CustomerName,
                    Discount = customer.Discount,
                    TaxRate = customer.TaxRate,
                    Email = customer.Email,
                    FaxNumber = customer.FaxNumber,
                    Id = customer.Id,
                    PaymentTermId = customer.PaymentTermId,
                    PhoneNumber = customer.PhoneNumber,
                    PriceLevelId = customer.PriceLevelId,
                    State = customer.State,
                    ZipCode = customer.ZipCode,
                    Zone = string.Empty, // zone == null ? "" : zone.BinCode
                    IsHoldAccount = customer.IsHoldAccount,
                    CreditLimit = customer.CreditLimit != null ? customer.CreditLimit.Value : 0,
                    IsBypassCreditLimit = customer.IsBypassCreditLimit,
                    OverBalance = customer.OverBalance != null ? customer.OverBalance.Value : 0
                };

                result.Add(customerDTO);
            }

            return result;
        }

        public async Task<PaginatedListDTO<CustomerDTO>> GetCustomersListPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "")
        {
            var customers = new List<Customer>();
            var customerDTOs = new List<CustomerDTO>();
            int recordCount = 0;

            if (string.IsNullOrEmpty(search))
            {
                recordCount = (from c in _context.Customers select c)
                .Distinct()
                .Count();

                if (sortOrder == "ASC")
                {
                    customers = await (from c in _context.Customers select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderBy(e => "e." + sortColumn)
                    //.OrderBy(e => e.CustomerName)
                }
                else
                {
                    customers = await (from c in _context.Customers select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderByDescending(e => "e." + sortColumn)
                    //.OrderByDescending(e => e.CustomerName)
                }
            }
            else
            {
                search = search.ToLower();
                recordCount = (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Count();

                if (sortOrder == "ASC")
                {
                    customers = await (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderBy(e => "e." + sortColumn)
                    //.OrderBy(e => e.CustomerName)
                }
                else
                {
                    customers = await (
                    from c in _context.Customers
                    where (c.IsDeleted == false && (c.CustomerName.Trim().ToLower().Contains(search) || c.PhoneNumber.Trim().ToLower().Contains(search) ||
                           c.AccountNumber.ToString().Contains(search) || c.AddressLine1.Contains(search)))
                    select c)
                    .Distinct()
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                    //.OrderByDescending(e => "e." + sortColumn)
                    //.OrderByDescending(e => e.CustomerName)
                }
            }

            foreach (var customer in customers)
            {
                //var zone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == customer.ZipCode.Trim().ToLower()).FirstOrDefaultAsync();
                var contacts = await _context.Contacts.Where(e => e.CustomerId == customer.Id && e.IsActive == true && e.IsDeleted == false).ToListAsync();
                var customerCredit = await _context.CustomerCredits.Where(e => e.CustomerId == customer.Id && e.IsActive == true).OrderBy(o => o.Id).FirstOrDefaultAsync();

                var customerDTO = new CustomerDTO()
                {
                    AccountNumber = customer.AccountNumber,
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    City = customer.City,
                    ContactName = customer.ContactName,
                    Country = customer.Country,
                    CustomerName = customer.CustomerName,
                    Discount = customer.Discount,
                    TaxRate = customer.TaxRate,
                    Email = customer.Email,
                    FaxNumber = customer.FaxNumber,
                    Id = customer.Id,
                    PaymentTermId = customer.PaymentTermId,
                    PhoneNumber = customer.PhoneNumber,
                    PriceLevelId = customer.PriceLevelId,
                    State = customer.State,
                    ZipCode = customer.ZipCode,
                    Zone = string.Empty, // zone == null ? "" : zone.BinCode
                    IsHoldAccount = customer.IsHoldAccount,
                    CreditLimit = customer.CreditLimit != null ? customer.CreditLimit.Value : 0,
                    IsBypassCreditLimit = customer.IsBypassCreditLimit,
                    OverBalance = customer.OverBalance != null ? customer.OverBalance.Value : 0,
                    Contacts = contacts,
                    CustomerCredit = customerCredit
                };

                customerDTOs.Add(customerDTO);
            }

            var result = new PaginatedListDTO<CustomerDTO>()
            {
                Data = customerDTOs,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<CustomerDTO>> GetReportCustomersListPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "", int? searchPaymentTermId = 0, string? searchState = "")
        {
            var customers = new List<Customer>();
            var customerDTOs = new List<CustomerDTO>();

            var recordCount = (
                from customer in _context.Customers
                join order in _context.Orders on customer.Id equals order.CustomerId
                where (customer.IsActive && !customer.IsDeleted && (searchPaymentTermId == 0 ? true : customer.PaymentTermId == searchPaymentTermId) &&
                       order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 3 && (order.Balance != 0) &&
                       ((string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.AccountNumber.ToString().Contains(search))) &&
                        (string.IsNullOrEmpty(searchState) ? true : customer.State.Trim().ToLower() == searchState.Trim().ToLower()))
                select customer)
                .Distinct()
                .Count();

            if (sortOrder == "ASC")
            {
                customers = await (
                from customer in _context.Customers
                join order in _context.Orders on customer.Id equals order.CustomerId
                where (customer.IsActive && !customer.IsDeleted && (searchPaymentTermId == 0 ? true : customer.PaymentTermId == searchPaymentTermId) &&
                        order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 3 && (order.Balance != 0) &&
                        ((string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.AccountNumber.ToString().Contains(search))) &&
                        (string.IsNullOrEmpty(searchState) ? true : customer.State.Trim().ToLower() == searchState.Trim().ToLower()))
                select customer)
                .Distinct()
                .OrderBy(e => e.CustomerName)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                customers = await (
                from customer in _context.Customers
                join order in _context.Orders on customer.Id equals order.CustomerId
                where (customer.IsActive && !customer.IsDeleted && (searchPaymentTermId == 0 ? true : customer.PaymentTermId == searchPaymentTermId) &&
                        order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 3 && (order.Balance != 0) &&
                        ((string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.PhoneNumber.Trim().ToLower().Contains(search.ToLower())) ||
                        (string.IsNullOrEmpty(search) ? true : customer.AccountNumber.ToString().Contains(search))))
                select customer)
                .Distinct()
                .OrderByDescending(e => e.CustomerName)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
            }

            foreach (var customer in customers)
            {
                //Select CUstomer order where balance not equal to zero
                //var customerOrder = await _context.Orders.FirstOrDefaultAsync(e => e.Balance != 0 && e.IsActive && !e.IsDeleted && !e.IsQuote && e.OrderStatusId != 3);
                //if (customerOrder != null)
                //{
                //var zone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == customer.ZipCode.Trim().ToLower()).FirstOrDefaultAsync();
                var contacts = await _context.Contacts.Where(e => e.CustomerId == customer.Id && e.IsActive == true && e.IsDeleted == false).ToListAsync();
                var customerCredit = await _context.CustomerCredits.Where(e => e.CustomerId == customer.Id && e.IsActive == true).OrderBy(o => o.Id).FirstOrDefaultAsync();

                var customerDTO = new CustomerDTO()
                {
                    AccountNumber = customer.AccountNumber,
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    City = customer.City,
                    ContactName = customer.ContactName,
                    Country = customer.Country,
                    CustomerName = customer.CustomerName,
                    Discount = customer.Discount,
                    TaxRate = customer.TaxRate,
                    Email = customer.Email,
                    FaxNumber = customer.FaxNumber,
                    Id = customer.Id,
                    PaymentTermId = customer.PaymentTermId,
                    PhoneNumber = customer.PhoneNumber,
                    PriceLevelId = customer.PriceLevelId,
                    State = customer.State,
                    ZipCode = customer.ZipCode,
                    Zone = string.Empty, // zone == null ? "" : zone.BinCode
                    IsHoldAccount = customer.IsHoldAccount,
                    CreditLimit = customer.CreditLimit != null ? customer.CreditLimit.Value : 0,
                    IsBypassCreditLimit = customer.IsBypassCreditLimit,
                    OverBalance = customer.OverBalance != null ? customer.OverBalance.Value : 0,
                    Contacts = contacts,
                    CustomerCredit = customerCredit
                };

                customerDTOs.Add(customerDTO);
                //}
            }

            var result = new PaginatedListDTO<CustomerDTO>()
            {
                Data = customerDTOs,
                RecordCount = recordCount
            };

            return result;
        }
        public async Task<Customer?> GetCustomer(int customerId)
        {
            var result = await _context.Customers.FindAsync(customerId);
            if (result == null)
                return null;
            return result;
        }

        public async Task<CustomerDTO?> GetCustomerById(int customerId)
        {
            var contacts = await _context.Contacts.Where(e => e.CustomerId == customerId && e.IsActive == true && e.IsDeleted == false).ToListAsync();
            var customerCredit = await _context.CustomerCredits.Where(e => e.CustomerId == customerId && e.IsActive == true).OrderBy(o => o.Id).FirstOrDefaultAsync();

            var result = await _context.Customers
                .Select(e => new CustomerDTO()
                {
                    Id = e.Id,
                    AccountNumber = e.AccountNumber,
                    CustomerName = e.CustomerName.Trim(),
                    Discount = e.Discount,
                    TaxRate = e.TaxRate,
                    PaymentTermId = e.PaymentTermId,
                    PriceLevelId = e.PriceLevelId,
                    AddressLine1 = e.AddressLine1,
                    AddressLine2 = e.AddressLine2,
                    City = e.City.Trim(),
                    Country = e.Country.Trim(),
                    Email = e.Email != null ? e.Email.Trim() : "",
                    ContactName = e.ContactName.Trim(),
                    FaxNumber = e.FaxNumber.Trim(),
                    PhoneNumber = e.PhoneNumber.Trim(),
                    State = e.State.Trim(),
                    ZipCode = e.ZipCode.Trim(),
                    IsHoldAccount = e.IsHoldAccount,
                    CreditLimit = e.CreditLimit != null ? e.CreditLimit.Value : 0,
                    IsBypassCreditLimit = e.IsBypassCreditLimit,
                    OverBalance = e.OverBalance != null ? e.OverBalance.Value : 0,
                    Contacts = contacts,
                    CustomerCredit = customerCredit
                })
                .Where(e => e.Id == customerId)
                .FirstOrDefaultAsync();

            if (result == null)
                return null;
            return result;
        }

        public async Task<List<CustomerEmailDTO>?> GetCustomerEmailsById(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == customerId);
            var location = await _context.Locations.FirstOrDefaultAsync(e => e.CustomerId == customerId && e.IsActive && !e.IsDeleted);

            if (customer != null)
            {
                var result = new List<CustomerEmailDTO>();

                if (!string.IsNullOrEmpty(customer.Email))
                {
                    result.Add(new CustomerEmailDTO()
                    {
                        CustomerId = customerId,
                        LocationId = location.Id,
                        PositionTypeId = 1,
                        ContactName = customer.ContactName.Trim(),
                        Email = customer.Email.Trim(),
                        PhoneNumber = customer.PhoneNumber.Trim(),
                    });
                }

                var contacts = await _context.Contacts.Where(e => e.CustomerId == customerId && e.IsActive == true && e.IsDeleted == false).ToListAsync();
                if (contacts == null) return result;

                foreach (var contact in contacts)
                {
                    if (!string.IsNullOrEmpty(contact.Email))
                    {
                        if (result.FindIndex(e => e.Email == contact.Email.Trim()) == -1)
                        {
                            result.Add(new CustomerEmailDTO()
                            {
                                ContactName = contact.ContactName.Trim(),
                                Email = contact.Email.Trim(),
                                PhoneNumber = contact.PhoneNumber.Trim(),
                            });
                        }
                    }
                }

                return result;
            }

            return null;
        }

        public async Task<CustomerDTO?> GetCustomerByAccountNumber(int accountNumber)
        {
            var result = await _context.Customers
                .Select(e => new CustomerDTO()
                {
                    Id = e.Id,
                    AccountNumber = e.AccountNumber,
                    CustomerName = e.CustomerName.Trim(),
                    Discount = e.Discount,
                    TaxRate = e.TaxRate,
                    PaymentTermId = e.PaymentTermId,
                    PriceLevelId = e.PriceLevelId,
                    AddressLine1 = e.AddressLine1,
                    AddressLine2 = e.AddressLine2,
                    City = e.City.Trim(),
                    Country = e.Country.Trim(),
                    Email = e.Email != null ? e.Email.Trim() : "",
                    ContactName = e.ContactName.Trim(),
                    FaxNumber = e.FaxNumber.Trim(),
                    PhoneNumber = e.PhoneNumber.Trim(),
                    State = e.State.Trim(),
                    ZipCode = e.ZipCode.Trim(),
                    IsHoldAccount = e.IsHoldAccount,
                    CreditLimit = e.CreditLimit != null ? e.CreditLimit.Value : 0,
                    IsBypassCreditLimit = e.IsBypassCreditLimit,
                    OverBalance = e.OverBalance != null ? e.OverBalance.Value : 0,
                    Contacts = new List<Contact>(),
                    CustomerCredit = new CustomerCredit(),
                    Locations = new List<Location>()
                })
                .Where(e => e.AccountNumber == accountNumber)
                .FirstOrDefaultAsync();


            if (result != null)
            {
                var contacts = await _context.Contacts.Where(e => e.CustomerId == result.Id && e.IsActive == true && e.IsDeleted == false).ToListAsync();
                var customerCredit = await _context.CustomerCredits.Where(e => e.CustomerId == result.Id && e.IsActive == true).OrderBy(o => o.Id).FirstOrDefaultAsync();
                var locations = await _context.Locations.Where(e => e.CustomerId == result.Id && e.IsActive && !e.IsDeleted).ToListAsync();

                result.Contacts = contacts;
                result.CustomerCredit = customerCredit;
                result.Locations = locations;

                return result;
            }

            return null;
        }

        public async Task<List<Location?>> GetCustomerLocations(int customerId)
        {
            var result = await _context.Locations.Where(c => c.CustomerId == customerId).ToListAsync();
            if (result == null)
                return null;
            return result;
        }
        #endregion

        #region Save Data
        public async Task<Customer> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveEntitiesAsync();

            var location = new Location()
            {
                AddressLine1 = customer.AddressLine1.Trim(),
                AddressLine2 = customer.AddressLine2,
                City = customer.City.Trim(),
                Country = customer.Country.Trim(),
                CreatedBy = customer.CreatedBy.Trim(),
                CreatedDate = customer.CreatedDate,
                CustomerId = customer.Id,
                Email = customer.Email,
                FaxNumber = customer.FaxNumber.Trim(),
                IsActive = customer.IsActive,
                IsDeleted = customer.IsDeleted,
                LocationCode = "BSL",
                LocationName = customer.CustomerName,
                LocationTypeId = 3,
                Notes = "Default Customer Location",
                PhoneNumber = customer.PhoneNumber.Trim(),
                State = customer.State.Trim(),
                ZipCode = customer.ZipCode.Trim()
            };

            _context.Locations.Add(location);
            await _context.SaveEntitiesAsync();

            bool setEmail = (customer.PaymentTermId == 4 || customer.PaymentTermId == 10 || customer.PaymentTermId == 11 || customer.PaymentTermId == 12 || customer.PaymentTermId == 19) ? true : false;
            var contact = new Contact()
            {
                ContactName = customer.ContactName.Trim(),
                CreatedBy = customer.CreatedBy.Trim(),
                CreatedDate = customer.CreatedDate,
                CustomerId = customer.Id,
                Email = customer.Email,
                IsActive = customer.IsActive,
                IsDeleted = customer.IsDeleted,
                LocationId = location.Id,
                Notes = "Default Customer Contact",
                PhoneNumber = customer.PhoneNumber.Trim(),
                PositionTypeId = 1,
                IsEmailCreditMemo = setEmail,
                IsEmailOrder = setEmail,
                IsEmailStatement = setEmail
            };
            
            _context.Contacts.Add(contact);
            await _context.SaveEntitiesAsync();

            return customer;
        }

        public async Task<bool> Update(Customer customer)
        {
            var baseLocation = await _context.Locations.Where(e => e.CustomerId == customer.Id && e.LocationTypeId == 3).FirstOrDefaultAsync();
            _context.Customers.Update(customer);
            
            if (baseLocation != null && MainLocationChanged(customer, baseLocation))
            {
                _context.Locations.Update(baseLocation);
            }

            return await _context.SaveEntitiesAsync();
        }

        private bool MainLocationChanged(Customer customer, Location baseLocation)
        {
            var result = false;
            
            if (customer.AddressLine1 != baseLocation.AddressLine1 || 
                customer.AddressLine2 != baseLocation.AddressLine2 ||
                customer.City != baseLocation.City ||
                customer.Country != baseLocation.Country ||
                customer.State != baseLocation.State ||
                customer.ZipCode != baseLocation.ZipCode)
            {
                baseLocation.AddressLine1 = customer.AddressLine1;
                baseLocation.AddressLine2 = customer.AddressLine2;
                baseLocation.City = customer.City;
                baseLocation.Country = customer.Country;
                baseLocation.State = customer.State;
                baseLocation.ZipCode = customer.ZipCode;
                result = true;
            }

            return result;
        }

        public async Task<List<Customer>> Delete(List<int> customerIds)
        {
            var customers = _context.Customers.Where(a => customerIds.Contains(a.Id)).ToList();
            _context.Customers.RemoveRange(customers);
            await _context.SaveEntitiesAsync();
            return new List<Customer>(); // await _context.Customers.ToListAsync();
        }

        public async Task<List<Customer>> SoftDelete(List<int> customerIds)
        {
            var customers = _context.Customers.Where(a => customerIds.Contains(a.Id)).ToList();
            customers.ForEach(c => { c.IsDeleted = true; });

            _context.Customers.UpdateRange(customers);
            await _context.SaveEntitiesAsync();
            return new List<Customer>(); // await _context.Customers.ToListAsync();
        }
        #endregion
    }
}
