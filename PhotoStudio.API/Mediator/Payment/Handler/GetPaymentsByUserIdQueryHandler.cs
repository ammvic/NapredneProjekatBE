using MediatR;
using PhotoStudio.API.Mediator.Payment.Query;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Payment.Handler
{
    public class GetPaymentsByUserIdQueryHandler : IRequestHandler<GetPaymentsByUserIdQuery, IEnumerable<PaymentDTO>>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsByUserIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentDTO>> Handle(GetPaymentsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetPaymentsByUserIdAsync(request.UserId);

            return payments.Select(payment => new PaymentDTO
            {
                Id = payment.Id,
                UserId = payment.UserId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status
            });
        }
    }
}
