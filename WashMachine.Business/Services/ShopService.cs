using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Business.Services
{
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

        public ShopModel GetShopModel(int id)
        {
            var entity = _dbContext.Shop.FirstOrDefault(c => c.Id == id);
            var model = _mapper.Map<ShopModel>(entity) ?? new ShopModel();

            if (entity != null)
            {
                entity.ShopComs = _dbContext.ShopCom.Where(c => c.ShopId == id).ToList();
                var dollarCom = entity.ShopComs.FirstOrDefault(w => w.ComType == "DollarCom");
                if (dollarCom != null)
                {
                    model.DollarCom = _mapper.Map<ShopComModel>(dollarCom);
                }
            }
            return model;
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
            Shop entity = _dbContext.Shop.FirstOrDefault(c => c.Id == model.Id);
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
            entity.MachineCode = model.MachineCode;
            entity.Location = model.Location;
            entity.ShopOwnerId = model.ShopOwnerId;

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
            DateTime currentTime = DateTime.Now;

            _dbContext.Log.Add(new Log() { Message = JsonConvert.SerializeObject(orderModel), Time = DateTime.Now });
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == orderModel.ShopCode);
            Order entity = _dbContext.Order.FirstOrDefault(w => w.PaymentId == orderModel.PaymentId && w.PaymentStatus == (int)PaymentStatus.InCompleted);

            if (entity != null)
            {
                entity.UpdateTime = currentTime;
                entity.PaymentStatus = (int)PaymentStatus.Completed;
                entity.Amount = orderModel.Amount;
                entity.DeviceId = orderModel.DeviceId;
                entity.CardJson = orderModel.CardJson;
                entity.OctopusNo = orderModel.OctopusNo;
                entity.Message = orderModel.Message;

                _dbContext.Order.Update(entity);
                _dbContext.SaveChanges();
            }
            else
            {
                entity = new Order
                {
                    ShopCode = orderModel.ShopCode,
                    ShopName = orderModel.ShopName,
                    Location = shop.Location,
                    Amount = orderModel.Amount,
                    Quantity = 1,
                    PaymentId = orderModel.PaymentId,
                    PaymentTypeId = orderModel.PaymentTypeId,
                    PaymentTypeName = orderModel.PaymentTypeName,
                    PaymentStatus = orderModel.PaymentStatus == (int)PaymentStatus.InCompleted ? (int)PaymentStatus.InCompleted : (int)PaymentStatus.Completed,
                    DeviceId = orderModel.DeviceId,
                    CardJson = orderModel.CardJson,
                    OctopusNo = orderModel.OctopusNo,
                    Message = orderModel.Message,
                    InsertTime = currentTime,
                    UpdateTime = currentTime,
                };

                _dbContext.Order.Add(entity);
                _dbContext.SaveChanges();
            }

            Payment payment = _dbContext.Payment.FirstOrDefault(w => w.Id == orderModel.PaymentId);
            if (payment != null)
            {
                payment.PaymentStatus = (int)PaymentStatus.Completed;
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

                Order entity = new Order
                {
                    ShopCode = payment.ShopCode,
                    ShopName = payment.ShopName,
                    Location = string.Empty,
                    Amount = payment.Amount,
                    Quantity = 1,
                    PaymentId = payment.Id,
                    PaymentTypeId = payment.PaymentTypeId,
                    PaymentTypeName = payment.PaymentTypeName,
                    PaymentStatus = (int)PaymentStatus.Cancel,
                    DeviceId = string.Empty,
                    CardJson = string.Empty,
                    Message = paymentModel.Message,
                    InsertTime = currentTime,
                    UpdateTime = currentTime,
                };

                _dbContext.Order.Add(entity);
                _dbContext.SaveChanges();
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

                if (payment.PaymentStatus == (int)PaymentStatus.Failed)
                {
                    Order entity = new Order
                    {
                        ShopCode = payment.ShopCode,
                        ShopName = payment.ShopName,
                        Location = string.Empty,
                        Amount = payment.Amount,
                        Quantity = 1,
                        PaymentId = payment.Id,
                        PaymentTypeId = payment.PaymentTypeId,
                        PaymentTypeName = payment.PaymentTypeName,
                        PaymentStatus = (int)PaymentStatus.Failed,
                        DeviceId = string.Empty,
                        CardJson = string.Empty,
                        Message = paymentModel.Message,
                        InsertTime = currentTime,
                        UpdateTime = currentTime,
                    };

                    _dbContext.Order.Add(entity);
                    _dbContext.SaveChanges();
                }
            }

            _dbContext.Update(payment);
            _dbContext.SaveChanges();
            PaymentModel model = _mapper.Map<PaymentModel>(payment);
            return model;
        }

        public OrderModel CreateIncompletedOrder(OrderModel orderModel)
        {
            DateTime currentTime = DateTime.Now;

            _dbContext.Log.Add(new Log() { Message = JsonConvert.SerializeObject(orderModel), Time = DateTime.Now });
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == orderModel.ShopCode);
            Order entity = _dbContext.Order.FirstOrDefault(w => w.PaymentId == orderModel.PaymentId && w.PaymentStatus == (int)PaymentStatus.InCompleted);

            if (entity == null)
            {
                entity = new Order
                {
                    ShopCode = orderModel.ShopCode,
                    ShopName = orderModel.ShopName,
                    Location = shop.Location,
                    Amount = orderModel.Amount,
                    Quantity = 1,
                    PaymentId = orderModel.PaymentId,
                    PaymentTypeId = orderModel.PaymentTypeId,
                    PaymentTypeName = orderModel.PaymentTypeName,
                    PaymentStatus = (int)PaymentStatus.InCompleted,
                    DeviceId = orderModel.DeviceId,
                    CardJson = orderModel.CardJson,
                    OctopusNo = orderModel.OctopusNo,
                    Message = orderModel.Message,
                    InsertTime = currentTime,
                    UpdateTime = currentTime,
                };

                _dbContext.Order.Add(entity);
                _dbContext.SaveChanges();
                Payment payment = _dbContext.Payment.FirstOrDefault(w => w.Id == orderModel.PaymentId);
                if (payment != null)
                {
                    payment.PaymentStatus = (int)PaymentStatus.Pending;
                }
                _dbContext.Update(payment);
            }

            var model = _mapper.Map<OrderModel>(entity);
            return model;
        }

        public List<ShopModel> GetShops()
        {
            List<ShopModel> shops = _mapper.Map<List<ShopModel>>(_dbContext.Shop.ToList());
            shops.ForEach((shop) =>
            {
                User user = _dbContext.User.FirstOrDefault(f => f.Id == shop.ShopOwnerId);
                shop.ShopOwner = user != null ? user.Email : string.Empty;
            });

            return shops;
        }
    }
}
