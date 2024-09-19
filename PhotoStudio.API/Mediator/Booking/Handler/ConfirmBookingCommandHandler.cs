using MediatR;
using PhotoStudio.API.Mediator.Booking.Command;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;

    public ConfirmBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        return await _bookingRepository.ConfirmBookingAsync(request.BookingId, request.AdvancePaymentAmount);
    }
}
