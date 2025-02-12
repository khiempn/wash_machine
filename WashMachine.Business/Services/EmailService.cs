using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace WashMachine.Business.Services
{
    public class EmailService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public EmailService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private EmailSettingModel GetEmailSetting()
        {
            SettingService service = new SettingService(_mapper, _dbContext, _httpContextAccessor);
            return service.GetEmailSetting();
        }

        public bool SendMail(EmailModel email)
        {
            try
            {
                EmailSettingModel emailSetting = GetEmailSetting();

                var message = new MailMessage();
                Attachment att = null;
                var attachs = !string.IsNullOrEmpty(email.Attachments) ? new List<string>(email.Attachments.Split(";")) : new List<string>();

                SmtpClient smtpClient = new SmtpClient(emailSetting.Host)
                {
                    Port = emailSetting.Port,
                    Credentials = new NetworkCredential(emailSetting.UserName, emailSetting.Password),
                    EnableSsl = true
                };

                message.To.Add(new MailAddress(email.To));
                if (!string.IsNullOrEmpty(email.Cc))
                {
                    List<string> emailCc = email.Cc.Split(',').ToList();
                    foreach (var item in emailCc)
                    {
                        message.CC.Add(new MailAddress(item));
                    }
                }

                if (!string.IsNullOrEmpty(emailSetting.Receiver))
                {
                    List<string> emailCc = emailSetting.Receiver.Split(',').ToList();
                    foreach (var item in emailCc)
                    {
                        message.CC.Add(new MailAddress(item));
                    }
                }

                if (attachs != null && attachs.Any())
                {
                    foreach (var file in attachs)
                    {
                        if (File.Exists(file))
                        {
                            att = new Attachment(file);
                            message.Attachments.Add(att);
                        }
                    }
                }
                message.From = new MailAddress(emailSetting.From);
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = true;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _dbContext.Log.Add(new Log() { Message = ex.Message });
                _dbContext.SaveChanges();
                return false;
            }
            return true;
        }

        public EmailTemplateModel GetEmailTemplate(string type)
        {
            EmailTemplate emailTemplate = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type.Equals(type));
            if (emailTemplate != null)
            {
                return new EmailTemplateModel()
                {
                    Template = emailTemplate.Template,
                    Type = emailTemplate.Type,
                    Subject = emailTemplate.Subject,
                };
            }

            return null;
        }
    }
}
