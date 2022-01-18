using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Command.CheckoutOrder
{
    public class CheckoutOrderCommand : IRequest<int>
    {

        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }

        // BillingAddress
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Payment
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public int PaymentMethod { get; set; }


        public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IEmailService _emailService;
            private readonly IMapper _mapper;
            private readonly ILogger<CheckoutOrderCommandHandler> _logger;

            public CheckoutOrderCommandHandler
                (IOrderRepository orderRepository, 
                IEmailService emailService, 
                IMapper mapper, 
                ILogger<CheckoutOrderCommandHandler> logger)
            {
                _orderRepository = orderRepository;
                _emailService = emailService;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
            {
                var orderEntity = _mapper.Map<Order>(request);
                var newOrder = await _orderRepository.AddAsync(orderEntity);
                _logger.LogInformation($"New order [id:{newOrder.Id}] successfully created.");
                //if(!await SendMail(newOrder))
                //{
                //    _logger.LogError($"SendMail failed.");
                //}
                return newOrder.Id;
            }

            private async Task<bool> SendMail(Order newOrder)
            {
                var email = new Email()
                {
                    To="plamenpentchev@yahoo.com",
                    Subject="New Order has been created.",
                    Body="New order has just been created."
                };
                try
                {
                    return await _emailService.SendMail(email);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Order failed due to an error in the mail service. {ex.Message}");
                    return false;
                }
               
            }
        }
    }
}
