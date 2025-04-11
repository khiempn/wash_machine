using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;

namespace WashMachine.Business.Services
{
    public class RunCommandErrorService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public RunCommandErrorService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<RunCommandErrorModel> GetRunCommandErrors()
        {
            return _mapper.Map<List<RunCommandErrorModel>>(_dbContext.RunCommandError.ToList());
        }

        public RunCommandErrorModel TrackingMachineError(RunCommandErrorModel runCommandErrorModel)
        {
            DateTime currentTime = DateTime.Now;
            Order order = _dbContext.Order.FirstOrDefault(f => f.PaymentId == runCommandErrorModel.PaymentId) ?? new Order();
            OrderModel orderModel = _mapper.Map<OrderModel>(order);

            RunCommandError entity = new RunCommandError
            {
                ShopCode = runCommandErrorModel.ShopCode,
                ShopName = runCommandErrorModel.ShopName,
                Command = runCommandErrorModel.Command,
                MachineName = runCommandErrorModel.MachineName,
                Amount = orderModel.Amount,
                OrderId = orderModel.Id,
                PaymentTypeName = orderModel.PaymentTypeName,
                OctopusNo = orderModel.OctopusNo,
                BuyerAccountID = orderModel.BuyerAccountID,
                InsertTime = currentTime,
                UpdateTime = currentTime,
            };

            _dbContext.RunCommandError.Add(entity);
            _dbContext.SaveChanges();

            RunCommandErrorModel model = _mapper.Map<RunCommandErrorModel>(entity);
            return model;
        }
    }
}
