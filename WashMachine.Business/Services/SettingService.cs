using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

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
            //if (systemInfo.User.Missions == null) systemInfo.User.Missions = new List<string>();


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

            return new ShopSettingModel()
            {
                CouponScanImgUrl = GetSetting(nameof(model.CouponScanPath)),
                AlipayScanImgUrl = GetSetting(nameof(model.AlipayScanPath)),
                OctopusScanImgUrl = GetSetting(nameof(model.OctopusScanPath)),
                PaymeScanImgUrl = GetSetting(nameof(model.PaymeScanPath)),
                AlipayPaymentImgUrl = GetSetting(nameof(model.AlipayPaymentnPath)),
                OctopusPaymentImgUrl = GetSetting(nameof(model.OctopusPaymentPath)),
                PaymePaymentImgUrl = GetSetting(nameof(model.PaymePaymentPath))
            };
        }
    }
}
