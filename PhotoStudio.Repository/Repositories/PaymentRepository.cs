using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoStudio.Infrastructure;

namespace PhotoStudio.Repository.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PhotoStudioContext _context;

        public PaymentRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        // Ova metoda simulira plaćanje i proverava da li je korisnik uneo dovoljno avansa.
        public async Task<bool> SimulatePayment(int bookingId, decimal amount)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return false;
            }

          
            return amount >= booking.AdvanceAmount;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

       
    }
}
