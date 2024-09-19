using MediatR;
using PhotoStudio.API.Mediator.Payment.Command;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Payment.Handler
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<PaymentCommand, bool>
    {
        private readonly IPaymentRepository _paymentRepository;

        public ConfirmPaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<bool> Handle(PaymentCommand request, CancellationToken cancellationToken)
        {
            // Simuliramo potvrdu uplata koristeći `SimulatePayment` metod
            var isPaymentSimulated = await _paymentRepository.SimulatePayment(request.BookingId, 0); // Amount se ovde ne koristi
            return isPaymentSimulated;
        }
    }
}
