using AutoMapper;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Exceptions;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class BookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> IsDateTimeAvailableAsync(BookingDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);
            return await _unitOfWork.Bookings.IsDateTimeAvailableAsync(booking);
        }

        public async Task<(string Offer, decimal Price)> GenerateOfferAsync(BookingDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);
            return await _unitOfWork.Bookings.GenerateOfferAsync(booking);
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, decimal advancePaymentAmount)
        {
            var result = await _unitOfWork.Bookings.ConfirmBookingAsync(bookingId, advancePaymentAmount);
            if (!result)
            {
                throw new BookingException("Unable to confirm booking.");
            }
            return result;
        }

        public async Task CreateBookingAsync(BookingDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);

            if (!await _unitOfWork.Bookings.IsDateTimeAvailableAsync(booking))
            {
                throw new BookingException("Selected date and time are not available.");
            }

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.CompleteAsync();
        }
    }
}
