using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.GraphDB.Services
{
    public class BookingGraphDbContext
    {
        private readonly IGraphClient _client;

        public BookingGraphDbContext(IGraphClient client)
        {
            _client = client;
        }

        // Provera da li je datum i vreme dostupno
        public async Task<bool> IsDateTimeAvailableAsync(Booking booking)
        {
            var result = await _client.Cypher
                .Match("(booking:Booking)")
                .Where((Booking b) => b.DateTime == booking.DateTime && b.EmployeeId == booking.EmployeeId)
                .Return(b => b.As<Booking>())
                .ResultsAsync;

            return !result.Any(); // Vraća true ako ne postoji booking sa istim datumom i vremenom
        }

        // Generisanje ponude na osnovu rezervacije
        public async Task<(string Offer, decimal Price)> GenerateOfferAsync(Booking booking)
        {
            // Primer: Generišemo ponudu i cenu na osnovu tipa usluge
            string offer = $"Offer for {booking.ServiceType} at {booking.Location}";
            decimal price = 100m; // Primer cene

            return (offer, price);
        }

        // Dohvatanje zaposlenog prema ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var result = await _client.Cypher
                .Match("(employee:Employee)")
                .Where((Employee employee) => employee.Id == employeeId)
                .Return(employee => employee.As<Employee>())
                .ResultsAsync;

            return result.FirstOrDefault(); 
        }

        // Potvrda rezervacije
        public async Task<bool> ConfirmBookingAsync(int bookingId, decimal advancePaymentAmount)
        {
            var result = await _client.Cypher
                .Match("(booking:Booking)")
                .Where((Booking booking) => booking.Id == bookingId)
                .Set("booking.Status = 'Confirmed', booking.AdvanceAmount = $advancePaymentAmount")
                .WithParam("advancePaymentAmount", advancePaymentAmount)
                .Return(booking => booking.As<Booking>())
                .ResultsAsync;

            return result.Any(); // Vraća true ako je rezervacija uspešno potvrđena
        }

        // Dodavanje rezervacije
        public async Task AddAsync(Booking booking)
        {
            await _client.Cypher
                .Create("(booking:Booking {newBooking})")
                .WithParam("newBooking", booking)
                .ExecuteWithoutResultsAsync();
        }
    }
}
