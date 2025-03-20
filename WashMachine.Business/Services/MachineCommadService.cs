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

        public MachineCommandModel GetAll()
        {
            List<MachineCommand> machineCommands = _dbContext.MachineCommand.ToList();
            MachineCommandModel commandModel = new MachineCommandModel();
            foreach (var machineCommand in machineCommands)
            {
                var field = commandModel.GetType().GetProperty(machineCommand.Key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                field.SetValue(commandModel, machineCommand.Value);
            }
            return commandModel;
        }

        public Respondent SaveAll(MachineCommandModel machineCommand)
        {
            List<MachineCommand> machineCommands = _dbContext.MachineCommand.ToList();
            if (machineCommands.Count > 0)
            {
                _dbContext.MachineCommand.RemoveRange(machineCommands);
                _dbContext.SaveChanges();
            }

            foreach (var propertyInfo in machineCommand.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(machineCommand, null)?.ToString();

                _dbContext.MachineCommand.Add(new MachineCommand()
                {
                    Key = propertyInfo.Name,
                    Value = value
                });
            }
            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "Machine Command")
            };
        }
    }
}
