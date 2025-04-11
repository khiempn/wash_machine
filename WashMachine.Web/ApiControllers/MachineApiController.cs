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
    public class MachineApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public MachineApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpPost]
        public Respondent TrackingMachineError([FromBody] RunCommandErrorModel runCommandErrorModel)
        {
            try
            {
                RunCommandErrorService runCommandErrorService = _business.GetService<RunCommandErrorService>();
                RunCommandErrorModel runCommandError = runCommandErrorService.TrackingMachineError(runCommandErrorModel);
                if (runCommandError != null)
                {
                    return new Respondent() { Success = true, Data = runCommandError };
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Can not add new a tracking." };
                }
            }
            catch (Exception ex)
            {
                return new Respondent() { Success = false, Message = ex.InnerException.Message };
            }
        }
    }
}
