using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WashMachine.Business.Services
{
    public class SettingService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;

        public SettingService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public SystemInfo GetSystemInfo(int userId)
        {
            var systemInfo = new SystemInfo();

            // 1.Get user
            if (userId == 0) userId = _httpContextAccessor.HttpContext.GetId();
            if (userId != 0)
            {
                var user = _dbContext.User.FirstOrDefault(c => c.Id == userId);
                systemInfo.User = _mapper.Map<UserModel>(user);
            }
            if (systemInfo.User == null) systemInfo.User = new UserModel();


            // 2.Get system setting
            var settingType = typeof(SettingModel);
            var settings = new SettingModel();
            var listSettings = _dbContext.Setting.ToList();
            foreach (var item in listSettings)
            {
                if (item == null) continue;
                if (item.Key == null) continue;
                var piInstance = settingType.GetProperty(item.Key);
                if (piInstance == null) continue;
                piInstance.SetValue(settings, item.Value);
            }

            settings.EmailTemplate = new Models.EmailTemplateConfig();
            var emailGenerationError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "GenerationError");
            if (emailGenerationError != null)
            {
                settings.EmailTemplate.GenerationErrorSubject = emailGenerationError.Subject;
                settings.EmailTemplate.GenerationErrorBody = emailGenerationError.Template;
            }

            var emailDisconnectError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "DisconnectError");
            if (emailDisconnectError != null)
            {
                settings.EmailTemplate.DisconnectErrorSubject = emailDisconnectError.Subject;
                settings.EmailTemplate.DisconnectErrorBody = emailDisconnectError.Template;
            }

            var emailUploadFileError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "UploadFileError");
            if (emailUploadFileError != null)
            {
                settings.EmailTemplate.UploadFileErrorSubject = emailUploadFileError.Subject;
                settings.EmailTemplate.UploadFileErrorBody = emailUploadFileError.Template;
            }

            var emailDownloadError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "DownloadError");
            if (emailDownloadError != null)
            {
                settings.EmailTemplate.DownloadErrorSubject = emailDownloadError.Subject;
                settings.EmailTemplate.DownloadErrorBody = emailDownloadError.Template;
            }
            systemInfo.Setting = settings;
            return systemInfo;
        }

        public Respondent SaveSetting(SettingModel model)
        {
            var response = new Respondent();

            SaveImageSetting(nameof(model.OctopusPaymentPath), model.OctopusPaymentFile);
            SaveImageSetting(nameof(model.PaymePaymentPath), model.PaymePaymentFile);
            SaveImageSetting(nameof(model.AlipayPaymentnPath), model.AlipayPaymentFile);

            SaveImageSetting(nameof(model.CouponScanPath), model.CouponScanFile);
            SaveImageSetting(nameof(model.OctopusScanPath), model.OctopusScanFile);
            SaveImageSetting(nameof(model.PaymeScanPath), model.PaymeScanFile);
            SaveImageSetting(nameof(model.AlipayScanPath), model.AlipayScanFile);

            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = Messages.SaveSuccess;
            return response;
        }

        public Respondent SaveOctopusSetting(SettingModel model)
        {
            var response = new Respondent();

            SetSetting(nameof(model.XFileHour), model.XFileHour);
            SetSetting(nameof(model.XFileMinute), model.XFileMinute);

            SetSetting(nameof(model.UploadHour), model.UploadHour);
            SetSetting(nameof(model.UploadMinute), model.UploadMinute);

            SetSetting(nameof(model.DownloadHour), model.DownloadHour);
            SetSetting(nameof(model.DownloadMinute), model.DownloadMinute);

            SetSetting(nameof(model.OctopusUploadFolder), model.OctopusUploadFolder);
            SetSetting(nameof(model.OctopusDownloadFolder), model.OctopusDownloadFolder);

            SetSetting(nameof(model.ServerEmailAddress), model.ServerEmailAddress);
            SetSetting(nameof(model.ServerEmailPassword), model.ServerEmailPassword);
            SetSetting(nameof(model.ServerEmailReceiver), model.ServerEmailReceiver);
            SetSetting(nameof(model.ServerEmailHost), model.ServerEmailHost);
            SetSetting(nameof(model.ServerEmailPort), model.ServerEmailPort);
            SetSetting(nameof(model.ServerEmailFrom), model.ServerEmailFrom);

            _dbContext.SaveChanges();

            var emailGenerationError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "GenerationError");
            if (emailGenerationError != null)
            {
                emailGenerationError.Subject = model.EmailTemplate.GenerationErrorSubject;
                emailGenerationError.Template = model.EmailTemplate.GenerationErrorBody;
                _dbContext.EmailTemplate.Update(emailGenerationError);
                _dbContext.SaveChanges();
            }

            var emailDisconnectError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "DisconnectError");
            if (emailDisconnectError != null)
            {
                emailDisconnectError.Subject = model.EmailTemplate.DisconnectErrorSubject;
                emailDisconnectError.Template = model.EmailTemplate.DisconnectErrorBody;
                _dbContext.EmailTemplate.Update(emailDisconnectError);
                _dbContext.SaveChanges();
            }

            var emailUploadFileError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "UploadFileError");
            if (emailUploadFileError != null)
            {
                emailUploadFileError.Subject = model.EmailTemplate.UploadFileErrorSubject;
                emailUploadFileError.Template = model.EmailTemplate.UploadFileErrorBody;
                _dbContext.EmailTemplate.Update(emailUploadFileError);
                _dbContext.SaveChanges();
            }

            var emailDownloadError = _dbContext.EmailTemplate.FirstOrDefault(f => f.Type == "DownloadError");
            if (emailDownloadError != null)
            {
                emailDownloadError.Subject = model.EmailTemplate.DownloadErrorSubject;
                emailDownloadError.Template = model.EmailTemplate.DownloadErrorBody;
                _dbContext.EmailTemplate.Update(emailDownloadError);
                _dbContext.SaveChanges();
            }

            response.Success = true;
            response.Message = Messages.SaveSuccess;
            return response;
        }

        private void SaveImageSetting(string key, IFormFile file)
        {
            if (file == null) return;
            var filePath = FormFileUtilities.SaveImage(file, Constants.UploadSettingFolder);
            SetSetting(key, filePath);
        }

        private Setting SetSetting(string key, string value)
        {
            var entity = _dbContext.Setting.FirstOrDefault(c => c.Key == key);
            if (entity == null)
            {
                entity = new Setting
                {
                    Key = key
                };
                _dbContext.Setting.Add(entity);
            }
            entity.Value = value;
            return entity;
        }

        private string GetSetting(string key)
        {
            var entity = _dbContext.Setting.FirstOrDefault(c => c.Key == key);
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Value))
            {
                return entity.Value;
            }
            return string.Empty;
        }

        public OctopusSettingModel GetOctopusSetting()
        {
            OctopusSettingModel model = new OctopusSettingModel();
            model.XFileHour = GetSetting(nameof(model.XFileHour));
            model.XFileMinute = GetSetting(nameof(model.XFileMinute));
            model.UploadHour = GetSetting(nameof(model.UploadHour));
            model.UploadMinute = GetSetting(nameof(model.UploadMinute));
            model.DownloadHour = GetSetting(nameof(model.DownloadHour));
            model.DownloadMinute = GetSetting(nameof(model.DownloadMinute));
            model.OctopusUploadFolder = GetSetting(nameof(model.OctopusUploadFolder));
            model.OctopusDownloadFolder = GetSetting(nameof(model.OctopusDownloadFolder));
            return model;
        }

        public EmailSettingModel GetEmailSetting()
        {
            SettingModel model = new SettingModel();

            EmailSettingModel emailSetting = new EmailSettingModel();
            emailSetting.UserName = GetSetting(nameof(model.ServerEmailAddress));
            emailSetting.Password = GetSetting(nameof(model.ServerEmailPassword));
            emailSetting.Receiver = GetSetting(nameof(model.ServerEmailReceiver));
            emailSetting.Host = GetSetting(nameof(model.ServerEmailHost));
            emailSetting.Port = Convert.ToInt32(GetSetting(nameof(model.ServerEmailPort)));
            emailSetting.From = GetSetting(nameof(model.ServerEmailFrom));
            if (string.IsNullOrEmpty(emailSetting.From))
            {
                emailSetting.From = emailSetting.UserName;
            }
            return emailSetting;
        }

        public ShopSettingModel GetGeneralSetting()
        {
            SettingModel model = new SettingModel();
            MachineCommadService machineCommadService = new MachineCommadService(_mapper, _dbContext, _httpContextAccessor);

            return new ShopSettingModel()
            {
                CouponScanImgUrl = GetSetting(nameof(model.CouponScanPath)),
                AlipayScanImgUrl = GetSetting(nameof(model.AlipayScanPath)),
                OctopusScanImgUrl = GetSetting(nameof(model.OctopusScanPath)),
                PaymeScanImgUrl = GetSetting(nameof(model.PaymeScanPath)),
                AlipayPaymentImgUrl = GetSetting(nameof(model.AlipayPaymentnPath)),
                OctopusPaymentImgUrl = GetSetting(nameof(model.OctopusPaymentPath)),
                PaymePaymentImgUrl = GetSetting(nameof(model.PaymePaymentPath)),
                MachineCommandConfig = machineCommadService.GetAll()
            };
        }
    }
}
