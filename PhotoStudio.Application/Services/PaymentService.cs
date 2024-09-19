using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Exceptions;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> CreatePaymentLink(int bookingId, decimal amount)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100), // Amount in cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Booking Payment"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://yourdomain.com/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://yourdomain.com/cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _unitOfWork.Payments.GetPaymentsByUserIdAsync(userId);
        }

        public async Task<bool> ConfirmPayment(int bookingId, string paymentConfirmationCode)
        {
            var service = new SessionService();
            var session = await service.GetAsync(paymentConfirmationCode);

            return session.PaymentStatus == "paid";
        }
    }
}
