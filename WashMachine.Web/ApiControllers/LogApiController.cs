using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WashMachine.Web.ApiControllers
{
    public class LogApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public LogApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpPost]
        public Respondent Log(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return new Respondent() { Success = false, Message = $"message is can not empty." };
                }

                LogService logService = _business.GetService<LogService>();
                var result = logService.Log(message);

                if (result.Success == false)
                {
                    return new Respondent() { Success = false, Message = $"Can not add log" };
                }
                else
                {
                    return new Respondent() { Success = true, DataId = 1 };

                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }
    }
}
