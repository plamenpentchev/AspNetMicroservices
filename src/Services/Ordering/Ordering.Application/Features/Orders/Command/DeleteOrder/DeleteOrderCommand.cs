using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Command.DeleteOrder
{
    public class DeleteOrderCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMapper _mapper;
            private readonly ILogger<DeleteOrderCommandHandler> _logger;

            public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
            {
                _orderRepository = orderRepository;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
            {
                var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);
                if (null == orderToDelete)
                {
                    //_logger.LogError($"Order [id:{request.Id}] could not be found. Cant not delete order.");
                    throw new NotFoundException(nameof(Order), request.Id);
                }
                await _orderRepository.DeleteAsync(orderToDelete);
                _logger.LogInformation($"Order [id:{request.Id}] succsessfully deleted.");
                return Unit.Value;
            }
        }
    }
}
