using DotLiquid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Libraries
{
    public static class EmailHelper
    {
        public static bool EmailDisabled = false;
        public static bool SendMail(EmailMessage model)
        {
            if (EmailDisabled) return true;

            var message = new MailMessage();
            Attachment att = null;
            var attachs = !string.IsNullOrEmpty(model.Attachments) ? new List<string>(model.Attachments.Split(";")) : new List<string>();

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(model.Email, model.Passsword),
                EnableSsl = true,
            };

            SmtpServer = new SmtpClient("smtp.mailgun.org")
            {
                Port = 587,
                Credentials = new NetworkCredential(model.Email, model.Passsword),
                EnableSsl = true,
            };
            message.To.Add(new MailAddress(model.To));
            if (!string.IsNullOrEmpty(model.Cc))
            {
                List<string> emailCc = model.Cc.Split(',').ToList();
                foreach (var item in emailCc)
                {
                    message.CC.Add(new MailAddress(item));
                }
            }
            if (!string.IsNullOrEmpty(model.Bcc))
            {
                List<string> emailBcc = model.Bcc.Split(',').ToList();
                foreach (var item in emailBcc)
                {
                    message.Bcc.Add(new MailAddress(item));
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
            message.From = new MailAddress(model.From);
            message.Subject = model.Subject;
            message.Body = model.Body;
            message.IsBodyHtml = true;
            SmtpServer.Send(message);
            return true;
        }
        
        public static void RegisterViewModel(Type rootType)
        {
            rootType
                .Assembly
                .GetTypes()
                .Where(t => t.Namespace == rootType.Namespace)
                .ToList()
                .ForEach(RegisterSafeTypeWithAllProperties);
        }

        public static void RegisterSafeTypeWithAllProperties(Type type)
        {
            Template.RegisterSafeType(type,
                type
                    .GetProperties()
                    .Select(p => p.Name)
                    .ToArray());
        }

        public static string RenderViewModel(this Template template, object root)
        {
            return template.Render(Hash.FromAnonymousObject(root));
        }
    }

    public class EmailMessage
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Passsword { get; set; }
        public string Email { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SendOn { get; set; }
        public string Attachments { get; set; }
    }

    public class EmailModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Domain { get; set; }
    }
}
