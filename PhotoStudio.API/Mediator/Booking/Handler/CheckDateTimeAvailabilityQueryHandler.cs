using MediatR;
using PhotoStudio.API.Mediator.Booking.Query;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

public class CheckDateTimeAvailabilityQueryHandler : IRequestHandler<CheckDateTimeAvailabilityQuery, bool>
{
    private readonly IBookingRepository _bookingRepository;

    public CheckDateTimeAvailabilityQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<bool> Handle(CheckDateTimeAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var booking = new Booking
        {
            DateTime = request.DateTime,
            ServiceId = request.ServiceId
        };

        return await _bookingRepository.IsDateTimeAvailableAsync(booking);
    }
}
