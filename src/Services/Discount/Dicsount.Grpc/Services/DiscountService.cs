using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using  Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    /// <summary>
    /// Exposes services the grpc way
    /// </summary>
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }


        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (null == coupon)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount to product name = '{request.ProductName} was not found.'"));
               
            }
            _logger.LogInformation($"Discount is retrieved for product '{request.ProductName}', amount '{coupon.Amount}'.");
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override  async Task<CouponModel> UpdateDiscoupt(UpdateDiscoutModel request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var updated = await _repository.UpdateDiscount(coupon);
            if (!updated)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to update the discount to product '{coupon.ProductName}'."));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }


        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var created = await _repository.CreateDiscount(coupon);
            if (!created)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to create new discount to product '{coupon.ProductName}'."));
            }
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            if (!deleted)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to delete the discount to product '{request.ProductName}'."));
            }

            return new DeleteDiscountResponse() { 
                Success = true
            };
        }
    }
}
