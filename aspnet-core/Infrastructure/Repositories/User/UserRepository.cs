using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel;
using Domain.DomainModel.Interface.User;
using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface.RolesAndAccess;

namespace Infrastructure.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IRoleRepository _roleRepository;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public UserRepository(DataContext context, IRoleRepository roleRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _roleRepository = roleRepository;
        }

        #region Get Data
        public async Task<List<Domain.DomainModel.Entity.User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Domain.DomainModel.Entity.User?> GetUserById(int userId)
        {
            var result = await _context.Users.FindAsync(userId);

            if (result != null)
            {
                result.Role = await _roleRepository.GetRole(result.RoleId);
            }

            return result;
        }

        public async Task<Domain.DomainModel.Entity.User?> GetUserByName(string userName)
        {
            var result = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();

            if (result != null)
            {
                result.Role = await _roleRepository.GetRole(result.RoleId);
            }

            return result;
        }

        public async Task<Domain.DomainModel.Entity.User?> GetUserByEmail(string email)
        {
            var result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            
            if (result != null )
            {
                result.Role = await _roleRepository.GetRole(result.RoleId);
            }
            
            return result;
        }

        public async Task<UserNotificationDTO> GetUserNotificationsByUserId(int userId, string creditMemoFilterDate)
        {
            DateTime filterDate = DateTime.Today.AddDays(-7);
            DateTime cmFilterDate = DateTime.Parse(creditMemoFilterDate);

            var result = new UserNotificationDTO();

            #region Get Follow Up
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user != null)
            {
                var currentOrders = await _context.Orders.Where(u => u.OrderDate.Date >= filterDate.Date && u.IsActive && !u.IsDeleted && !u.IsQuote).GroupBy(e => e.CustomerId)
                    .Select(c => c.OrderByDescending(d => d.OrderDate).First()).ToListAsync();

                var currentCustomerIds = currentOrders.Select(e => e.CustomerId).ToList();

                var orders = await _context.Orders.Where(u => !currentCustomerIds.Contains(u.CustomerId) && u.OrderDate.Date < filterDate.Date && u.IsActive && !u.IsDeleted && !u.IsQuote).GroupBy(e => e.CustomerId)
                    .Select(c => c.OrderByDescending(d => d.OrderDate).First()).ToListAsync();

                // Get orders by User
                orders = orders.Where(e => e.CreatedBy.Trim() == user.UserName.Trim()).ToList();
                var customerIds = orders.Select(e => e.CustomerId).ToList();

                var customerNotes = await _context.CustomerNotes.Where(n => customerIds.Contains(n.CustomerId)).GroupBy(e => e.CustomerId)
                    .Select(c => c.OrderByDescending(d => d.CreatedDate).First()).ToListAsync();
                
                // Get customer notes within 7 days
                customerNotes = customerNotes.Where(e => e.CreatedDate.Date >= filterDate.Date).ToList();
                
                // Remove customer from order list where customer note is within 7 days
                foreach (var customerNote in customerNotes)
                {
                    var order = orders.FirstOrDefault(o => o.CustomerId == customerNote.CustomerId);
                    if (order != null)
                    {
                        orders.Remove(order);
                    }
                }

                foreach (var order in orders)
                {
                    var contact = await _context.Contacts.Where(c => c.CustomerId == order.CustomerId && c.PositionTypeId == 1).FirstOrDefaultAsync();
                    var followUp = new UserNotificationFollowUpDTO()
                    {
                        Email = order.Email,
                        CustomerName = order.CustomerName,
                        CustomerId = order.CustomerId,
                        PhoneNumber = order.PhoneNumber,
                        UserId = user.Id
                    };

                    if (contact != null)
                    {
                        followUp.PhoneNumber = contact.PhoneNumber;
                        followUp.Email = contact.Email;
                    }
                    
                    result.FollowUpList.Add(followUp);
                }
            }
            #endregion

            #region Get Credit Note
            if (user != null)
            {

                // Get orders by User
                var orders = await _context.Orders.Where(e => e.OrderStatusId == 5 && e.OrderDate.Date >= cmFilterDate.Date && e.CreatedBy.Trim() == user.UserName.Trim()).ToListAsync();
                var orderIds = orders.Select(e => e.Id).ToList();

                var orderNotes = await _context.OrderNotes.Where(n => orderIds.Contains(n.OrderId)).GroupBy(e => e.OrderId)
                    .Select(c => c.OrderByDescending(d => d.CreatedDate).First()).ToListAsync();

                // Remove order from list with order note 
                foreach (var orderNote in orderNotes)
                {
                    var order = orders.FirstOrDefault(o => o.Id == orderNote.OrderId);
                    if (order != null)
                    {
                        orders.Remove(order);
                    }
                }

                foreach (var order in orders)
                {
                    var contact = await _context.Contacts.Where(c => c.CustomerId == order.CustomerId && c.PositionTypeId == 1).FirstOrDefaultAsync();
                    var creditNote = new UserNotificationCreditNoteDTO()
                    {
                        Amount = order.TotalAmount,
                        CreditMemoNumber = order.OrderNumber,
                        Email = order.Email,
                        CustomerName = order.CustomerName,
                        CustomerId = order.CustomerId,
                        PhoneNumber = order.PhoneNumber,
                        UserId = user.Id,
                        OrderId = order.Id
                    };

                    if (contact != null)
                    {
                        creditNote.PhoneNumber = contact.PhoneNumber;
                        creditNote.Email = contact.Email;
                    }

                    result.CreditNoteList.Add(creditNote);
                }
            }
            #endregion

            return result;
        }

        public Task<Domain.DomainModel.Entity.User?> Login(UserDTO user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Save Data
        public async Task<Domain.DomainModel.Entity.User?> Create(Domain.DomainModel.Entity.User user)
        {
            _context.Users.Add(user);
            await _context.SaveEntitiesAsync();
            return await _context.Users.Where(c => c.Id == user.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Domain.DomainModel.Entity.User>> Update(Domain.DomainModel.Entity.User user)
        {
            _context.Users.Update(user);
            await _context.SaveEntitiesAsync();
            return await _context.Users.ToListAsync(); //Where(c => c.CustomerId == user.CustomerId).ToListAsync();
        }

        public async Task<List<Domain.DomainModel.Entity.User>> Delete(List<int> userIds)
        {
            var users = _context.Users.Where(a => userIds.Contains(a.Id)).ToList();
            _context.Users.RemoveRange(users);
            await _context.SaveEntitiesAsync();
            return await _context.Users.Where(c => c.CustomerId == users[0].CustomerId).ToListAsync();
        }

        public async Task<List<Domain.DomainModel.Entity.User>> SoftDelete(List<int> userIds)
        {
            var users = _context.Users.Where(a => userIds.Contains(a.Id)).ToList();
            users.ForEach(c => { c.IsDeleted = true; });

            _context.Users.UpdateRange(users);
            await _context.SaveEntitiesAsync();
            return await _context.Users.ToListAsync();
        }

        public async Task<Domain.DomainModel.Entity.User?> Register(Domain.DomainModel.Entity.User user)
        {
            _context.Users.Add(user);
            await _context.SaveEntitiesAsync();
            return await _context.Users.Where(c => c.CustomerId == user.CustomerId).FirstOrDefaultAsync();
        }
        #endregion
    }
}
