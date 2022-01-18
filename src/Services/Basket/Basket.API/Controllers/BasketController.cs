using AutoMapper;
using Basket.API.Entitites;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiVersion("1.0")]
    public class BasketController: BaseAPIController
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController
            (IBasketRepository repository, 
            DiscountGrpcService discountService, 
            IMapper mapper, 
            IPublishEndpoint eventBus)
        {
            _repository = repository;
            _discountService = discountService;
            _mapper = mapper;
            _publishEndpoint = eventBus;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO (1): connect to the grpc service

            //TODO (2): calculate end price .

            foreach (var item in basket.Items)
            {
                var coupon = await _discountService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            
            return Ok(await _repository.UpdateBasket(basket));
        }
        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get existing basket (to user name) with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (null == basket)
            {
                return BadRequest();
            }

            //create basketCheckouEvent-- set TotalPrice on basketCheckouEvent message
            var basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

            //send the checkout event to rabbitmq using MassTransit
            basketCheckoutEvent.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(basketCheckoutEvent);

            //remove the basket
            await _repository.DeleteBasket(basket.UserName);
            return Accepted();
        }

    }
}
