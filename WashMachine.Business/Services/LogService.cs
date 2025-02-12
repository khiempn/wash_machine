using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using System;

namespace WashMachine.Business.Services
{
    public class LogService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;

        public LogService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Respondent Log(string message)
        {
            _dbContext.Log.Add(new Log()
            {
                Message = message,
                Time = DateTime.Now
            });
            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "Log")
            };
        }
    }
}
