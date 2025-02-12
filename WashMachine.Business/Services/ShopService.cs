using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Business.Services
{
    public enum OrderTypes
    {
        PaymentHK = 1,
        TicketRedeem = 2
    }

    public class ShopService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public ShopService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public DataList<ShopModel> FindShops(ParamFilter filter)
        {
            var userId = _httpContextAccessor.HttpContext.GetId();
            var user = _dbContext.User.FirstOrDefault(c => c.Id == userId);

            var query = _dbContext.Shop.Where(c => c.Name.Contains(filter.Keyword)
                || c.Code.Contains(filter.Keyword)
                || c.Email.Contains(filter.Keyword)
                || c.Phone.Contains(filter.Keyword)
                || string.IsNullOrEmpty(filter.Keyword));
            if (user.UserType != (int)UserTypes.Admin)
            {
                var listShopIds = _dbContext.ShopOwner.Where(c => c.OwnerId == userId && c.Active == true).Select(s => s.ShopId).ToList();
                query = query.Where(c => listShopIds.Contains(c.Id));
            }
            var count = query.Count();
            var listItems = query.OrderByDescending(o => o.Id).ToList();

            var modelItems = _mapper.Map<List<ShopModel>>(listItems);
            var model = new DataList<ShopModel>(modelItems, filter.Page, filter.PageSize, count);
            return model;
        }

        public ShopModel GetShopModel(int id)
        {
            var entity = _dbContext.Shop.FirstOrDefault(c => c.Id == id);
            var model = _mapper.Map<ShopModel>(entity) ?? new ShopModel();
            var userIds = _dbContext.ShopOwner.Where(c => c.ShopId == id && c.Active != false).Select(s => s.OwnerId).ToList();
            var listUsers = _dbContext.User.Where(c => userIds.Contains(c.Id)).ToList();
            model.ShopUsers = _mapper.Map<List<UserModel>>(listUsers);
            model.UserIds = "-0-" + string.Join("-", userIds) + "-";

            if (entity != null)
            {
                entity.ShopComs = _dbContext.ShopCom.Where(c => c.ShopId == id).ToList();
                var dollarCom = entity.ShopComs.FirstOrDefault(w => w.ComType == "DollarCom");
                if (dollarCom != null)
                {
                    model.DollarCom = _mapper.Map<ShopComModel>(dollarCom);
                }

                var couponCom = entity.ShopComs.FirstOrDefault(w => w.ComType == "CouponCom");
                if (couponCom != null)
                {
                    model.CouponCom = _mapper.Map<ShopComModel>(couponCom);
                }

                var octopusCom = entity.ShopComs.FirstOrDefault(w => w.ComType == "OctopusCom");
                if (octopusCom != null)
                {
                    model.OctopusCom = _mapper.Map<ShopComModel>(octopusCom);
                }
            }
            return model;
        }

        public Respondent SaveShopowner(ShopModel model)
        {
            var response = new Respondent();
            var userIds = model.UserIds.Split('-', StringSplitOptions.RemoveEmptyEntries);
            var listUserIds = userIds.Select(c => TextUtilities.GetInt(c)).ToList();

            var listShopUsers = _dbContext.ShopOwner.Where(c => c.ShopId == model.Id).ToList();
            var removedUsers = listShopUsers.Where(c => c.ShopId == model.Id && !listUserIds.Contains(c.OwnerId)).ToList();
            foreach (var item in removedUsers)
            {
                item.Active = false;
            }
            foreach (var item in listUserIds)
            {
                if (item == 0) continue;
                var obj = listShopUsers.FirstOrDefault(c => c.OwnerId == item);
                if (obj == null)
                {
                    obj = new ShopOwner
                    {
                        ShopId = model.Id,
                        OwnerId = item
                    };
                    _dbContext.Add(obj);
                }
                obj.Active = true;
            }
            _dbContext.SaveChanges();

            response.Success = true;
            return response;
        }

        public Respondent SaveShop(ShopModel model)
        {
            var response = new Respondent();
            var checkItem = _dbContext.Shop.FirstOrDefault(c => c.Id != model.Id && c.Code == model.Code);
            if (checkItem != null)
            {
                response.Message = string.Format(Messages.DataExisted, "This code");
                return response;
            }
            var currentTime = DateTime.Now;
            var entity = _dbContext.Shop.FirstOrDefault(c => c.Id == model.Id);
            if (entity == null)
            {
                entity = new Shop
                {
                    InsertTime = currentTime,
                    Code = model.Code
                };
                _dbContext.Shop.Add(entity);
            };
            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.ShortName = model.ShortName;
            entity.Address = model.Address;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.Notes = model.Notes;
            entity.Location = model.Location;
            entity.MachineCode = model.MachineCode;

            var userId = _httpContextAccessor.HttpContext.GetId();
            var user = _dbContext.User.FirstOrDefault(c => c.Id == userId);
            if (user.UserType == (int)UserTypes.Admin)
            {
                if (model.BackgroundFile != null)
                {
                    entity.BackgroundPath = FormFileUtilities.SaveImage(model.BackgroundFile, Constants.ShopFolder);
                }
                if (model.LogoFile != null)
                {
                    entity.LogoPath = FormFileUtilities.SaveImage(model.LogoFile, Constants.ShopFolder);
                }
                entity.Status = model.Status;
            }
            entity.UpdateTime = currentTime;
            entity.ShopComs.Clear();
            _dbContext.SaveChanges();

            var shopComs = _dbContext.ShopCom.Where(w => w.ShopId == entity.Id);
            foreach (var shopCom in shopComs)
            {
                _dbContext.ShopCom.Remove(shopCom);
            }

            _dbContext.ShopCom.Add(new ShopCom()
            {
                ComType = model.DollarCom.ComType,
                BaudRate = model.DollarCom.BaudRate,
                ComName = model.DollarCom.ComName,
                Data = model.DollarCom.Data,
                Parity = model.DollarCom.Parity,
                ShopId = model.Id,
                StopBits = model.DollarCom.StopBits
            });
            _dbContext.ShopCom.Add(new ShopCom()
            {
                ComType = model.CouponCom.ComType,
                BaudRate = model.CouponCom.BaudRate,
                ComName = model.CouponCom.ComName,
                Data = model.CouponCom.Data,
                Parity = model.CouponCom.Parity,
                ShopId = model.Id,
                StopBits = model.CouponCom.StopBits
            });
            _dbContext.ShopCom.Add(new ShopCom()
            {
                ComType = model.OctopusCom.ComType,
                BaudRate = model.OctopusCom.BaudRate,
                ComName = model.OctopusCom.ComName,
                Data = model.OctopusCom.Data,
                Parity = model.OctopusCom.Parity,
                ShopId = model.Id,
                StopBits = model.OctopusCom.StopBits
            });
            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "Shop")
            };
        }

        public ShopModel GetShopByCode(string code)
        {
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (shop != null)
            {
                var shopModel = _mapper.Map<ShopModel>(shop);
                return shopModel;
            }
            return null;
        }

        public PaymentModel CreateNewPayment(PaymentModel paymentModel)
        {
            var currentTime = DateTime.Now;
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == paymentModel.ShopCode);

            var entity = new Payment
            {
                ShopCode = paymentModel.ShopCode,
                ShopName = shop.Name,
                PaymentTypeId = paymentModel.PaymentTypeId,
                PaymentTypeName = paymentModel.PaymentTypeName,
                Amount = paymentModel.Amount,
                InsertTime = currentTime,
                UpdateTime = currentTime,
                PaymentStatus = paymentModel.PaymentStatus,
                Message = paymentModel.Message
            };

            _dbContext.Payment.Add(entity);
            _dbContext.SaveChanges();
            var model = _mapper.Map<PaymentModel>(entity);
            return model;
        }

        public OrderModel CreateNewOrder(OrderModel orderModel)
        {
            _dbContext.Log.Add(new Log() { Message = JsonConvert.SerializeObject(orderModel), Time = DateTime.Now });
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == orderModel.ShopCode);

            DateTime currentTime = DateTime.Now;
            Order entity = new Order
            {
                ShopCode = orderModel.ShopCode,
                ShopName = orderModel.ShopName,
                Location = shop.Location,
                Amount = orderModel.Amount,
                Quantity = 1,
                PaymentId = orderModel.PaymentId,
                PaymentTypeId = orderModel.PaymentTypeId,
                PaymentTypeName = orderModel.PaymentTypeName,
                PaymentStatus = (int)PaymentStatus.Paid,
                DeviceId = orderModel.DeviceId,
                CardJson = orderModel.CardJson,
                InsertTime = currentTime,
                UpdateTime = currentTime,
            };

            _dbContext.Order.Add(entity);
            _dbContext.SaveChanges();

            Payment payment = _dbContext.Payment.FirstOrDefault(w => w.Id == orderModel.PaymentId);
            if (payment != null)
            {
                payment.PaymentStatus = (int)PaymentStatus.Paid;
            }
            _dbContext.Update(payment);

            var model = _mapper.Map<OrderModel>(entity);
            return model;
        }

        public PaymentModel CancelPayment(PaymentModel paymentModel)
        {
            DateTime currentTime = DateTime.Now;
            Payment payment = _dbContext.Payment.FirstOrDefault(w => w.Id == paymentModel.Id);
            if (payment != null && payment.PaymentStatus == (int)PaymentStatus.Pending)
            {
                payment.PaymentStatus = (int)PaymentStatus.Cancel;
                payment.UpdateTime = currentTime;
                payment.Message = paymentModel.Message;
            }
            _dbContext.Update(payment);
            _dbContext.SaveChanges();
            var model = _mapper.Map<PaymentModel>(payment);
            return model;
        }

        public PaymentModel UpdatePayment(PaymentModel paymentModel)
        {
            DateTime currentTime = DateTime.Now;
            Payment payment = _dbContext.Payment.FirstOrDefault(w => w.Id == paymentModel.Id);
            if (payment != null)
            {
                payment.PaymentStatus = paymentModel.PaymentStatus;
                payment.UpdateTime = currentTime;
                payment.Message = paymentModel.Message;
            }
            _dbContext.Update(payment);
            _dbContext.SaveChanges();
            PaymentModel model = _mapper.Map<PaymentModel>(payment);
            return model;
        }

        public List<ShopModel> GetShops()
        {
            return _mapper.Map<List<ShopModel>>(_dbContext.Shop.ToList());
        }
    }
}
