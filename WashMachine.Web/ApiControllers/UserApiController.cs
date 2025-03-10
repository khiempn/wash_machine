using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WashMachine.Web.ApiControllers
{
    public class UserApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public UserApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpPost]
        public Respondent SignIn(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new Respondent() { Success = false, Message = $"The User Name can not empty." };
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return new Respondent() { Success = false, Message = $"The Password can not empty." };
                }

                AccessService service = _business.GetService<AccessService>();
                UserModel user = service.GetUserModel(username);
                if (user == null)
                {
                    return new Respondent() { Success = false, Message = $"The user does not exists." };
                }

                string encodePassword = TextUtilities.Encryption(password, null);
                if (encodePassword != user.Password)
                {
                    return new Respondent() { Success = false, Message = $"The password is incorrect." };
                }

                if (user.TestingMode == 0)
                {
                    return new Respondent() { Success = false, Message = $"You have no permission to access." };
                }

                return new Respondent() { Success = true };
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }
    }
}
