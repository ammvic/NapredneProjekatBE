using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> SimulatePayment(int bookingId, decimal amount);
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);
    }
}
