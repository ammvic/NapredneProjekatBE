using MediatR;

namespace PhotoStudio.API.Mediator.Payment.Command
{
    public class PaymentCommand : IRequest<bool>
    {
        public int BookingId { get; }
        public string PaymentConfirmationCode { get; }

        public PaymentCommand(int bookingId, string paymentConfirmationCode)
        {
            BookingId = bookingId;
            PaymentConfirmationCode = paymentConfirmationCode;
        }
    }
}
