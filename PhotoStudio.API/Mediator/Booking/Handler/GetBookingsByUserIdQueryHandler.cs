using MediatR;
using AutoMapper;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.API.Mediator.Booking.Query;
using PhotoStudio.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Booking.Handler
{
    public class GetBookingsByUserIdQueryHandler : IRequestHandler<GetBookingsQuery, IEnumerable<BookingDTO>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingsByUserIdQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }
    }
}
