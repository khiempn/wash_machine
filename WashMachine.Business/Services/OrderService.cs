using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Business.Services
{
    public class OrderService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public OrderService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
    
        public List<OrderModel> GetOrders()
        {
            return _mapper.Map<List<OrderModel>>(_dbContext.Order.ToList());
        }
    }
}
