using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    public class DiscountsController: BaseAPIController
    {
        private readonly IDiscountRepository _repository;

        public DiscountsController(IDiscountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet(Name = "GetDiscounts")]
        [ProducesResponseType(type: typeof(IEnumerable<Coupon>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetDiscounts()
        {
            var coupon = await _repository.GetDiscounts();
            return Ok(coupon);
            //return 0 == coupon.Amount ? NotFound() :Ok(coupon);
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(type:typeof(Coupon), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _repository.GetDiscount(productName);
            return Ok(coupon);
            //return 0 == coupon.Amount ? NotFound() :Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(type: typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(type:typeof(Coupon), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody]Coupon coupon)
        {
            await _repository.CreateDiscount(coupon);
            return CreatedAtRoute("GetDiscount", new { productName=coupon.ProductName} ,coupon);
        }

        [HttpPut]
        [ProducesResponseType(type: typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> UpdateDiscount([FromBody] Coupon coupon)
        {
            return Ok(await _repository.UpdateDiscount(coupon));
        }

        [HttpDelete("{productName}", Name ="DeleteDiscount")]
        [ProducesResponseType(type: typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
            return Ok(await _repository.DeleteDiscount(productName));
        }
    }
}
