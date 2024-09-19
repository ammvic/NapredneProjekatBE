using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.Booking.Query
{
    public class GenerateOfferQuery : IRequest<(string Offer, decimal Price)>
    {
        public int BookingId { get; }

        public GenerateOfferQuery(int bookingId)
        {
            BookingId = bookingId;
        }
    }

}
