using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WashMachine.Web.ApiControllers
{
    public class ShopApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public ShopApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpGet]
        public Respondent CheckExistShopCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                ShopService shopService = _business.GetService<ShopService>();
                ShopModel shopModel = shopService.GetShopByCode(code);

                if (shopModel == null || shopModel.Status == null || shopModel.Status == 0)
                {
                    return new Respondent() { Success = false, Message = $"The shop code {code} does not exists." };
                }
                else
                {
                    return new Respondent() { Success = true };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpGet]
        public Respondent SignIn(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                ShopService shopService = _business.GetService<ShopService>();
                ShopModel shopModel = shopService.GetShopByCode(code);

                if (shopModel == null || shopModel.Status == null || shopModel.Status == 0)
                {
                    return new Respondent() { Success = false, Message = $"The shop code {code} does not exists." };
                }
                else
                {
                    return new Respondent() { Success = true, Data = shopModel };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent CreateNewPayment([FromBody] PaymentModel paymentModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentModel.ShopCode))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                ShopService shopService = _business.GetService<ShopService>();
                PaymentModel payment = shopService.CreateNewPayment(paymentModel);
                if (payment != null)
                {
                    return new Respondent() { Success = true, Data = payment };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not create new payment." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }

        [HttpPost]
        public Respondent CompletePayment([FromBody]OrderModel orderModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderModel.ShopCode))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                ShopService shopService = _business.GetService<ShopService>();

                OrderModel order = shopService.CreateNewOrder(orderModel);
                if (order != null)
                {
                    return new Respondent() { Success = true, Message = string.Empty, Data = order };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not create new payment." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }

        [HttpPost]
        public Respondent CancelPayment([FromBody] PaymentModel paymentModel)
        {
            try
            {
                ShopService shopService = _business.GetService<ShopService>();
                PaymentModel payment = shopService.CancelPayment(paymentModel);
                if (payment != null)
                {
                    return new Respondent() { Success = true, Data = payment };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not create new payment." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }

        [HttpPost]
        public Respondent UpdatePayment([FromBody] PaymentModel paymentModel)
        {
            try
            {
                ShopService shopService = _business.GetService<ShopService>();
                PaymentModel payment = shopService.UpdatePayment(paymentModel);
                if (payment != null)
                {
                    return new Respondent() { Success = true, Data = payment };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not create new payment." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }

        [HttpGet]
        public Respondent GetSetting(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                SettingService settingService = _business.GetService<SettingService>();
                ShopSettingModel shopSettingModel = settingService.GetGeneralSetting();
                if (shopSettingModel == null)
                {
                    return new Respondent() { Success = false, Message = $"The shop code {code} does not exists." };
                }
                else
                {
                    return new Respondent() { Success = true, Data = shopSettingModel };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent CreateIncompletedPayment([FromBody] OrderModel orderModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderModel.ShopCode))
                {
                    return new Respondent() { Success = false, Message = $"The shop code can not empty." };
                }

                ShopService shopService = _business.GetService<ShopService>();

                OrderModel order = shopService.CreateIncompletedOrder(orderModel);
                if (order != null)
                {
                    return new Respondent() { Success = true, Message = string.Empty, Data = order };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not create new payment." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }
    }
}
