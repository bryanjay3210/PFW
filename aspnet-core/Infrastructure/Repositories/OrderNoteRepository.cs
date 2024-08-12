using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastucture.Repositories
{
    public class OrderNoteRepository : IOrderNoteRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public OrderNoteRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<OrderNote>> GetOrderNotes()
        {
            return await _context.OrderNotes.ToListAsync();
        }

        public async Task<OrderNote?> GetOrderNote(int orderNoteId)
        {
            var result = await _context.OrderNotes.FindAsync(orderNoteId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<OrderNote>> GetOrderNotesByOrderId(int orderId)
        {
            var result = new List<OrderNote>();
            result = await _context.OrderNotes.Where(e => e.OrderId == orderId).OrderByDescending(e => e.CreatedDate).ToListAsync();
            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<OrderNote>> Create(OrderNote orderNote)
        {
            await _context.OrderNotes.AddAsync(orderNote);
            await _context.SaveEntitiesAsync();
            return await _context.OrderNotes.Where(e => e.OrderId == orderNote.OrderId).OrderByDescending(e => e.CreatedDate).ToListAsync();
        }

        public async Task<List<OrderNote>> Update(OrderNote orderNote)
        {
            _context.OrderNotes.Update(orderNote);
            await _context.SaveEntitiesAsync();
            return await _context.OrderNotes.Where(e => e.OrderId == orderNote.OrderId).ToListAsync();
        }

        public async Task<List<OrderNote>> Delete(List<int> orderNoteIds)
        {
            var orderNotes = _context.OrderNotes.Where(a => orderNoteIds.Contains(a.Id)).ToList();
            _context.OrderNotes.RemoveRange(orderNotes);
            await _context.SaveEntitiesAsync();
            return await _context.OrderNotes.ToListAsync();
        }

        public async Task<List<OrderNote>> SoftDelete(List<int> orderNoteIds)
        {
            var orderNotes = _context.OrderNotes.Where(a => orderNoteIds.Contains(a.Id)).ToList();
            orderNotes.ForEach(cn => { cn.IsDeleted = true; });

            _context.OrderNotes.UpdateRange(orderNotes);
            await _context.SaveEntitiesAsync();
            return await _context.OrderNotes.ToListAsync();
        }
        #endregion
    }
}
