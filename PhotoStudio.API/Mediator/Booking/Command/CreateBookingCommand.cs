using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.Booking.Command
{
    public class CreateBookingCommand : IRequest<BookingDTO>
    {
        public int UserId { get; }
        public string ServiceType { get; }
        public string Location { get; }
        public int ServiceId { get; }
        public DateTime DateTime { get; }
        public bool IsAdditionalShootingIncluded { get; }

        public CreateBookingCommand(int userId, string serviceType, string location, int serviceId, DateTime dateTime, bool isAdditionalShootingIncluded)
        {
            UserId = userId;
            ServiceType = serviceType;
            Location = location;
            ServiceId = serviceId;
            DateTime = dateTime;
            IsAdditionalShootingIncluded = isAdditionalShootingIncluded;
        }
    }
}
