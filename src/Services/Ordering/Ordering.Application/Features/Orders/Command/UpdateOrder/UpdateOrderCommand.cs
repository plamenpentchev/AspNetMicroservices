using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Command.UpdateOrder
{
    public class UpdateOrderCommand: IRequest
    {
        public int Id { get; set; }
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

        public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly ILogger<UpdateOrderCommandHandler> _logger;

            public UpdateOrderCommandHandler
                (IOrderRepository orderRepository, 
                IMapper mapper, 
                ILogger<UpdateOrderCommandHandler> logger)
            {
                _orderRepository = orderRepository;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
            {
                var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
                if (null == orderToUpdate)
                {
                    //_logger.LogError($"There is no order with this id [{request.Id}]");
                    throw new NotFoundException(nameof(Order), request.Id);
                }
                _mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));

                await _orderRepository.UpdateAsync(orderToUpdate);

                _logger.LogInformation($"Order [id:{request.Id} successfully updated.");

                return Unit.Value;
            }
        }
    }
}
