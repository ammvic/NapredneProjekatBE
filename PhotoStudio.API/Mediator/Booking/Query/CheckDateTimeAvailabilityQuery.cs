using MediatR;
using PhotoStudio.Application.DTOs;
using System.Collections.Generic;

namespace PhotoStudio.API.Mediator.Booking.Query
{
    public class CheckDateTimeAvailabilityQuery : IRequest<bool>
    {
        public DateTime DateTime { get; }
        public int ServiceId { get; }

        public CheckDateTimeAvailabilityQuery(DateTime dateTime, int serviceId)
        {
            DateTime = dateTime;
            ServiceId = serviceId;
        }
    }

}
