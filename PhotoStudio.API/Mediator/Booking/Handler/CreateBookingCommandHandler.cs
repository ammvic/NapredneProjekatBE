using MediatR;
using PhotoStudio.API.Mediator.Booking.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDTO>
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingDTO> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new Booking
        {
            UserId = request.UserId,
            ServiceType = request.ServiceType,
            Location = request.Location,
            ServiceId = request.ServiceId,
            DateTime = request.DateTime,
            IsAdditionalShootingIncluded = request.IsAdditionalShootingIncluded,
            Status = "Pending",
            AdvanceAmount = 0 
        };

        await _bookingRepository.AddAsync(booking);

        return new BookingDTO
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ServiceType = booking.ServiceType,
            Location = booking.Location,
            ServiceId = booking.ServiceId,
            DateTime = booking.DateTime,
            Status = booking.Status,
            AdvanceAmount = booking.AdvanceAmount
        };
    }
}
