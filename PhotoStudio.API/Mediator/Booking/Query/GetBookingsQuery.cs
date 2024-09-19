using MediatR;
using PhotoStudio.Application.DTOs;
using System.Collections.Generic;

namespace PhotoStudio.API.Mediator.Booking.Query
{
    public class GetBookingsQuery : IRequest<IEnumerable<BookingDTO>>
    {
        public int UserId { get; }

        public GetBookingsQuery(int userId)
        {
            UserId = userId;
        }
    }

}
