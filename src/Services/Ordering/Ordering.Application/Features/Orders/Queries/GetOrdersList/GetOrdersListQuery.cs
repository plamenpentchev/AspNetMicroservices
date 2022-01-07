using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQuery : IRequest<List<OrdersVm>>
    {
        public string UserName { get; set; }

        public GetOrdersListQuery(string userName)
        {
            UserName = userName;
        }
        public class GetOrdersListHandler : IRequestHandler<GetOrdersListQuery, List<OrdersVm>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly ILogger<GetOrdersListHandler> _logger;
            private readonly IMapper _mapper;

            public GetOrdersListHandler
                (IOrderRepository orderRepository, ILogger<GetOrdersListHandler> logger, IMapper mapper)
            {
                _orderRepository = orderRepository;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<List<OrdersVm>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
            {
                var orders = await _orderRepository.GetOrdersByUserName(request.UserName);
                return _mapper.Map<List<OrdersVm>>(orders);
            }
        }
    }
}
