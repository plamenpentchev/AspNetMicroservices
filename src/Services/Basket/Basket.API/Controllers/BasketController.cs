using Basket.API.Entitites;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiVersion("1.0")]
    public class BasketController: BaseAPIController
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountService;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountService)
        {
            _repository = repository;
            _discountService = discountService;
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

    }
}
