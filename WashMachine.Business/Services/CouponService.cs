using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Business.Services
{
    public class CouponService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public CouponService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Respondent AddCoupon(CouponModel model)
        {
            Respondent response = new Respondent();
            Coupon checkItem = _dbContext.Coupon.FirstOrDefault(c => c.Code == model.Code);
            if (checkItem != null)
            {
                response.Message = string.Format(Messages.DataExisted, "This code");
                return response;
            }

            string shopName = string.Empty;

            if (_dbContext.Shop.Any(w => w.Code == model.ShopCode))
            {
                shopName = _dbContext.Shop.First(w => w.Code == model.ShopCode).Name;
            }

            _dbContext.Coupon.Add(new Coupon()
            {
                Code = model.Code,
                Discount = model.Discount,
                IsUsed = model.IsUsed,
                ShopCode = model.ShopCode,
                UsedDate = model.UsedDate,
                ShopName = shopName
            });

            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "Coupon")
            };
        }

        public CouponModel GetCouponByCode(string code)
        {
            Coupon coupon = _dbContext.Coupon.FirstOrDefault(c => c.Code == code);
            return _mapper.Map<CouponModel>(coupon);
        }

        public List<CouponModel> GetCoupons()
        {
            return _mapper.Map<List<CouponModel>>(_dbContext.Coupon);
        }
    }
}
