using AutoMapper;
using DotLiquid;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Web.ApiControllers
{
    public class EmailApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;
        private readonly WashMachineContext _dbContext;
        public EmailApiController(IMapper mapper, WashMachineContext dbContext, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
            _dbContext = dbContext;
        }

        [HttpPost]
        public Respondent SendGenerationError([FromBody] ErrorMailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("GenerationError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent SendDisconnectError([FromBody] ErrorMailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("DisconnectError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent SendUploadFileError([FromBody] ErrorMailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("UploadFileError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent SendDownloadError([FromBody]ErrorMailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("DownloadError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent SendEmailHealthCheckError([FromBody] MachineEmailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("HealthCheckError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent SendEmailStartError([FromBody] MachineEmailModel model)
        {
            try
            {
                var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == model.ShopCode);
                if (shop == null)
                {
                    return new Respondent() { Success = false, Message = "Shop code is not existed." };
                }

                if (string.IsNullOrEmpty(shop.Email))
                {
                    return new Respondent() { Success = false, Message = "This shop has not an email." };
                }

                model.Name = shop.Name;
                model.ShopName = shop.Name;

                var service = _business.GetService<EmailService>();
                EmailTemplateModel emailTemplate = service.GetEmailTemplate("StartError");
                if (emailTemplate != null)
                {
                    var template = Template.Parse(emailTemplate.Template);
                    var body = template.RenderViewModel(model);
                    Business.Models.EmailModel emailModel = new Business.Models.EmailModel
                    {
                        Body = body,
                        To = shop.Email,
                        Subject = emailTemplate.Subject
                    };

                    var result = service.SendMail(emailModel);
                    if (result)
                    {
                        return new Respondent() { Success = true, Message = string.Empty };
                    }
                    else
                    {
                        return new Respondent() { Success = false, Message = "Can not send email, please check your config." };
                    }
                }
                else
                {
                    return new Respondent() { Success = false, Message = "Email template is not found." };
                }
            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }
    }
}
