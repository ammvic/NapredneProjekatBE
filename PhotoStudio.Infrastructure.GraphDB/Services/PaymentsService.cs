using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.GraphDB.Services
{
    public class PaymentsGraphDbContext
    {
        private readonly IGraphClient _client;

        public PaymentsGraphDbContext(IGraphClient client)
        {
            _client = client;
        }

        // Kreiranje linka za uplatu
        public async Task<string> CreatePaymentLink(int bookingId, decimal amount)
        {
            string paymentLink = $"https://paymentgateway.com/pay?bookingId={bookingId}&amount={amount}";

            var payment = new Payment
            {
                UserId = bookingId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                Status = "Pending"
            };

            await _client.Cypher
                .Create("(payment:Payment {newPayment})")
                .WithParam("newPayment", payment)
                .ExecuteWithoutResultsAsync(); 

            return paymentLink;
        }

        // Potvrda uplate
        public async Task<bool> ConfirmPayment(int bookingId, string paymentConfirmationCode)
        {
            await _client.Cypher
                .Match("(payment:Payment)")
                .Where((Payment payment) => payment.UserId == bookingId && payment.Status == "Pending")
                .Set("payment.Status = 'Confirmed', payment.PaymentDate = $paymentDate")
                .WithParam("paymentDate", DateTime.UtcNow)
                .ExecuteWithoutResultsAsync(); 

            var updatedPayment = await _client.Cypher
                .Match("(payment:Payment)")
                .Where((Payment payment) => payment.UserId == bookingId && payment.Status == "Confirmed")
                .Return(payment => payment.As<Payment>())
                .ResultsAsync; 

            return updatedPayment.Any();
        }

        // Dohvatanje uplata na osnovu ID-a korisnika
        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            var result = await _client.Cypher
                .Match("(payment:Payment)")
                .Where((Payment payment) => payment.UserId == userId)
                .Return(payment => payment.As<Payment>())
                .ResultsAsync; 

            return result;
        }
    }
}
