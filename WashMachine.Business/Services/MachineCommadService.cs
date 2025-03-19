using AutoMapper;
using Libraries;
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
    public class MachineCommadService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public MachineCommadService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<MachineCommandModel> GetAll(int id)
        {
            List<MachineCommand> machineCommands = _dbContext.MachineCommand.ToList();
            return _mapper.Map<List<MachineCommandModel>>(machineCommands);
        }

        public Respondent SaveAll(List<MachineCommandModel> machineCommands)
        {
            _dbContext.MachineCommand.RemoveRange();
            foreach (MachineCommandModel machineCommand in machineCommands)
            {
                _dbContext.MachineCommand.Add(new MachineCommand()
                {
                    Key = machineCommand.Key,
                    Value = machineCommand.Value,
                });
            }

            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "Shop")
            };
        }
    }
}
