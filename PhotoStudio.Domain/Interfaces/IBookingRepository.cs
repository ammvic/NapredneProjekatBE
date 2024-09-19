using Microsoft.EntityFrameworkCore;
using PhotoStudio.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<Booking> GetBookingByIdAsync(int bookingId);

        Task<bool> IsDateTimeAvailableAsync(Booking booking);
        Task<(string Offer, decimal Price)> GenerateOfferAsync(Booking booking);
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<bool> ConfirmBookingAsync(int bookingId, decimal advancePaymentAmount);
        Task<(Booking booking, Employee photographer, Employee videographer)> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task<bool> CancelBooking(Guid bookingId);
        Task DeleteAsync(int bookingId);
    }
}

