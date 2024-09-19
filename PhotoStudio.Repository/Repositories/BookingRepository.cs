using Microsoft.EntityFrameworkCore;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using PhotoStudio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoStudio.Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly PhotoStudioContext _context;

        public BookingRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDateTimeAvailableAsync(Booking booking)
        {
            // Proveri da li postoji rezervacija za isti datum, vreme i lokaciju
            return !await _context.Bookings.AnyAsync(b =>
                b.DateTime == booking.DateTime &&
                b.Location == booking.Location &&
                b.ServiceType == booking.ServiceType);
        }

        public async Task<(string Offer, decimal Price)> GenerateOfferAsync(Booking booking)
        {
            // Na osnovu tipa usluge i lokacije generiši ponudu i cenu
            decimal price = 100;
            string offer = $"Ponuda za {booking.ServiceType} na lokaciji {booking.Location}";

            return (offer, price);
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, decimal advancePaymentAmount)
        {
            // Pronalaženje rezervacije po ID-u
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking != null && advancePaymentAmount > 0)
            {
                // Oznaka da je uplata izvršena i rezervacija potvrđena
                booking.Status = "Confirmed";

                // Ažuriranje statusa u bazi
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<(Booking booking, Employee photographer, Employee videographer)> AddAsync(Booking booking)
        {
            // Pronalaženje dostupnog fotografa
            var photographer = await _context.Employees
                .Where(e => e.Role == Role.Photographer)
                .FirstOrDefaultAsync(e => !e.EmployeeBookings.Any(eb => eb.Booking.DateTime == booking.DateTime));

            if (photographer == null)
            {
                throw new Exception("No available photographer found for the selected date and time.");
            }

            // Postavljanje podataka o fotografu u booking
            booking.EmployeeId = photographer.Id;
            booking.EmployeeFirstName = photographer.FirstName;
            booking.EmployeeLastName = photographer.LastName;

            // Kreiranje liste EmployeeBookings i dodavanje fotografa
            booking.EmployeeBookings = new List<EmployeeBooking>
    {
        new EmployeeBooking
        {
            EmployeeId = photographer.Id,
            Role = Role.Photographer
        }
    };

            Employee videographer = null;

            if (booking.IsAdditionalShootingIncluded)
            {
                // Pronalaženje dostupnog kamermana
                videographer = await _context.Employees
                    .Where(e => e.Role == Role.Cameraman)
                    .FirstOrDefaultAsync(e => !e.EmployeeBookings.Any(eb => eb.Booking.DateTime == booking.DateTime));

                if (videographer == null)
                {
                    throw new Exception("No available videographer found for the selected date and time.");
                }

                // Dodavanje kamermana u EmployeeBookings
                booking.EmployeeBookings.Add(new EmployeeBooking
                {
                    EmployeeId = videographer.Id,
                    Role = Role.Cameraman
                });
            }

            // Čuvanje booking-a zajedno sa EmployeeBookings
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return (booking, photographer, videographer);
        }





        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees.FindAsync(employeeId); 
        }
        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CancelBooking(Guid bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false;
            }

            booking.Status = "Canceled"; // Pretpostavljam da postoji polje Status koje menja stanje rezervacije
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }
        public async Task DeleteAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }



    }
}