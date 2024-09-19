using MediatR;
using PhotoStudio.Application.DTOs;
using System.Collections.Generic;

namespace PhotoStudio.API.Mediator.Payment.Query
{
    public class GetPaymentsByUserIdQuery : IRequest<IEnumerable<PaymentDTO>>
    {
        public int UserId { get; }

        public GetPaymentsByUserIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
