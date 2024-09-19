using MediatR;

namespace PhotoStudio.API.Mediator.Booking.Command
{
    public class ConfirmBookingCommand : IRequest<bool>
    {
        public int BookingId { get; }
        public decimal AdvancePaymentAmount { get; }

        public ConfirmBookingCommand(int bookingId, decimal advancePaymentAmount)
        {
            BookingId = bookingId;
            AdvancePaymentAmount = advancePaymentAmount;
        }
    }

}
