using MediatR;
using PhotoStudio.API.Mediator.Payment.Command;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Payment.Handler
{
    public class CreatePaymentLinkCommandHandler : IRequestHandler<CreatePaymentLinkCommand, string>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentLinkCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<string> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            // Simuliramo kreiranje linka za uplatu koristeći `SimulatePayment` metod
            var isPaymentSimulated = await _paymentRepository.SimulatePayment(request.BookingId, request.Amount);
            if (isPaymentSimulated)
            {
                // Simuliramo generisanje linka kao što smo pomenuli ranije
                return $"https://payment.example.com/{request.BookingId}?amount={request.Amount}";
            }
            return null;
        }
    }
}
