using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoBookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;
        private readonly IMongoCollection<EmployeeBooking> _employeebookingsCollection;
        private readonly IMongoCollection<Employee> _employeesCollection;

            public MongoBookingRepository(IMongoDatabase database)
            {
                _bookingsCollection = database.GetCollection<Booking>("bookings");
                _employeebookingsCollection = database.GetCollection<EmployeeBooking>("employeebookings");
                _employeesCollection = database.GetCollection<Employee>("employees");
            }


        public async Task<bool> IsDateTimeAvailableAsync(Booking booking)
        {
            var existingBooking = await _bookingsCollection
                .Find(b => b.DateTime == booking.DateTime &&
                           b.Location == booking.Location &&
                           b.ServiceType == booking.ServiceType)
                .FirstOrDefaultAsync();
            return existingBooking == null;
        }

        public async Task<(string Offer, decimal Price)> GenerateOfferAsync(Booking booking)
        {
            decimal price = await GeneratePriceAsync(booking);

            string offer = $"Service: {booking.ServiceType}\n" +
                           $"Location: {booking.Location}\n" +
                           $"Date & Time: {booking.DateTime}\n" +
                           $"Employee: {booking.EmployeeFirstName} {booking.EmployeeLastName}\n" +
                           $"Total Price: ${price}\n";

            return (offer, price);
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, decimal advancePaymentAmount)
        {
            var booking = await _bookingsCollection.Find(b => b.Id == bookingId).FirstOrDefaultAsync();
            if (booking == null)
            {
                return false;
            }

            booking.AdvanceAmount = advancePaymentAmount;
            await _bookingsCollection.ReplaceOneAsync(b => b.Id == bookingId, booking);
            return true;
        }

        public async Task<(Booking booking, Employee photographer, Employee videographer)> AddAsync(Booking booking)
        {
            // Insert the booking
            await _bookingsCollection.InsertOneAsync(booking);

            // Find available photographers and videographers
            var availablePhotographers = await _employeesCollection
                .Find(e => e.Role == Role.Photographer)
                .ToListAsync();

            Employee photographer = availablePhotographers
                .FirstOrDefault(e => !booking.EmployeeBookings.Any(eb => eb.EmployeeId == e.Id));

            if (photographer != null)
            {
                booking.EmployeeBookings.Add(new EmployeeBooking
                {
                    BookingId = booking.Id,
                    EmployeeId = photographer.Id,
                    Role = Role.Photographer
                });
            }

            Employee videographer = null;
            if (booking.IsAdditionalShootingIncluded)
            {
                var availableVideographers = await _employeesCollection
                    .Find(e => e.Role == Role.Cameraman)
                    .ToListAsync();

                videographer = availableVideographers
                    .FirstOrDefault(e => !booking.EmployeeBookings.Any(eb => eb.EmployeeId == e.Id));

                if (videographer != null)
                {
                    booking.EmployeeBookings.Add(new EmployeeBooking
                    {
                        BookingId = booking.Id,
                        EmployeeId = videographer.Id,
                        Role = Role.Cameraman
                    });
                }
            }

            // Update the booking with employees
            await _bookingsCollection.ReplaceOneAsync(b => b.Id == booking.Id, booking);

            return (booking, photographer, videographer);
        }

        private async Task<decimal> GeneratePriceAsync(Booking booking)
        {
            decimal basePrice = 0;

            switch (booking.ServiceType.ToLower())
            {
                case "photo shoot":
                    basePrice = 100m;
                    break;
                case "video shoot":
                    basePrice = 200m;
                    break;
                case "design":
                    basePrice = 150m;
                    break;
                default:
                    basePrice = 50m;
                    break;
            }

            if (booking.IsAdditionalShootingIncluded)
            {
                basePrice += 50m;
            }

            if (booking.Location.ToLower() == "remote")
            {
                basePrice += 30m;
            }

            return basePrice;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeesCollection.Find(e => e.Id == employeeId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _bookingsCollection.Find(b => b.UserId == userId).ToListAsync();
        }
        public async Task UpdateAsync(Booking booking)
        {
            var filter = Builders<Booking>.Filter.Eq(b => b.Id, booking.Id);
            await _bookingsCollection.ReplaceOneAsync(filter, booking);
        }
        public async Task<bool> CancelBooking(Guid bookingId)
        {
            var filter = Builders<Booking>.Filter.Eq("Id", bookingId); // Koristi naziv polja kao string
            var update = Builders<Booking>.Update.Set("Status", "Canceled"); // Koristi naziv polja kao string

            var result = await _bookingsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            var filter = Builders<Booking>.Filter.Eq("Id", bookingId); // Koristi naziv polja kao string

            return await _bookingsCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task DeleteAsync(int bookingId)
        {
            // Pronađi rezervaciju po njenom int ID-u
            var booking = await _bookingsCollection.Find(b => b.Id == bookingId).FirstOrDefaultAsync();

            if (booking != null)
            {
                // Prvo obriši sve povezane zapise u EmployeeBooking kolekciji koristeći int BookingId
                var employeeBookingFilter = Builders<EmployeeBooking>.Filter.Eq(eb => eb.BookingId, bookingId);
                await _employeebookingsCollection.DeleteManyAsync(employeeBookingFilter);

                // Zatim obriši rezervaciju iz Booking kolekcije koristeći int ID
                var bookingFilter = Builders<Booking>.Filter.Eq(b => b.Id, bookingId);
                await _bookingsCollection.DeleteOneAsync(bookingFilter);
            }
        }





    }

}
