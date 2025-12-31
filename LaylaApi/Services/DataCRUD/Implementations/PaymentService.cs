using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly LaylaContext _context;

        public PaymentService(LaylaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync() =>
            await _context.Payments.Include(p => p.Booking).ToListAsync();

        public async Task<Payment?> GetByIdAsync(int id) =>
            await _context.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Payment?> GetByBookingIdAsync(int bookingId) =>
            await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);

        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> UpdateAsync(int id, Payment payment)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return null;

            existing.Amount = payment.Amount;
            existing.Method = payment.Method;
            existing.Status = payment.Status;
            existing.TransactionId = payment.TransactionId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return false;

            _context.Payments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
