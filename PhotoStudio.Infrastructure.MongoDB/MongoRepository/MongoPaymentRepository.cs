using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoPaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _paymentsCollection;

        public MongoPaymentRepository(IMongoDatabase database)
        {
            _paymentsCollection = database.GetCollection<Payment>("payments");
        }

        // Simulacija uplate
        public async Task<bool> SimulatePayment(int bookingId, decimal amount)
        {
            // Kreiraj fiktivnu uplatu za simulaciju
            var simulatedPayment = new Payment
            {
                UserId = bookingId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                Status = "Simulated"
            };

            await _paymentsCollection.InsertOneAsync(simulatedPayment);

            return true; // Vraća true za uspešnu simulaciju
        }

        // Potvrda uplate na osnovu ID-a uplate
        public async Task<bool> ConfirmPayment(int bookingId, string paymentId)
        {
            var payment = await _paymentsCollection
                .Find(p => p.UserId == bookingId && p.Status == "Simulated")
                .FirstOrDefaultAsync();

            if (payment == null)
            {
                return false;
            }

            // Pretpostavljamo da za simulaciju samo ažuriramo status na "Confirmed"
            payment.Status = "Confirmed";
            await _paymentsCollection.ReplaceOneAsync(p => p.Id == payment.Id, payment);

            return true;
        }

        // Dobijanje uplata na osnovu ID-a korisnika
        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _paymentsCollection.Find(p => p.UserId == userId).ToListAsync();
        }
    }
}
