using MediatR;
using PhotoStudio.API.Mediator.Booking.Query;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, IEnumerable<BookingDTO>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(request.UserId);

        return bookings.Select(booking => new BookingDTO
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ServiceType = booking.ServiceType,
            Location = booking.Location,
            ServiceId = booking.ServiceId,
            DateTime = booking.DateTime,
            Status = booking.Status,
            AdvanceAmount = booking.AdvanceAmount
        });
    }
}
