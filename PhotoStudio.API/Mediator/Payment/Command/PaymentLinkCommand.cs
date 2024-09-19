using MediatR;

namespace PhotoStudio.API.Mediator.Payment.Command
{
    public class CreatePaymentLinkCommand : IRequest<string>
    {
        public int BookingId { get; }
        public decimal Amount { get; }

        public CreatePaymentLinkCommand(int bookingId, decimal amount)
        {
            BookingId = bookingId;
            Amount = amount;
        }
    }
}
