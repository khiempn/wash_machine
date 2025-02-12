using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WashMachine.Web.ApiControllers
{
    public class CouponApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public CouponApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpPost]
        public Respondent AddCouponUsed(string couponCode, string shopCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return new Respondent() { Success = false, Message = $"The coupon code can not empty." };
                }

                if (string.IsNullOrWhiteSpace(shopCode))
                {
                    return new Respondent() { Success = false, Message = $"The shopCode code can not empty." };
                }

                CouponService couponService = _business.GetService<CouponService>();
                var result = couponService.AddCoupon(new CouponModel() { Code = couponCode, UsedDate = DateTime.Now, ShopCode = shopCode, IsUsed = true, Discount = 20 , });

                if (result.Success == false)
                {
                    return new Respondent() { Success = false, Message = $"Can not add coupon used" };
                }
                else
                {
                    return new Respondent() { Success = true, DataId = 1 };

                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        public Respondent CheckCouponUsed(string couponCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return new Respondent() { Success = false, Message = $"The coupon code can not empty." };
                }

                CouponService couponService = _business.GetService<CouponService>();
                CouponModel result = couponService.GetCouponByCode(couponCode);

                if (result != null)
                {
                    return new Respondent() { Success = false, Message = "The coupon has been used." };
                }
                else
                {
                    return new Respondent() { Success = true, Message = $"The coupon is available." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }
    }
}
