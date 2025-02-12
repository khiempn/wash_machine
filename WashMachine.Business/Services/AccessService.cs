using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WashMachine.Business.Services
{
    public class AccessService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public AccessService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public UserModel GetUserModel(string email)
        {
            var entity = _dbContext.User.FirstOrDefault(c => c.Email == email);
            var model = _mapper.Map<UserModel>(entity);
            if(model != null)
            {
                model.ListShops = new List<Libraries.SelectItem>
                {

                };
            }
            return model;
        }

        public DataList<UserModel> FindUsers(ParamFilter filter)
        {
            var query = _dbContext.User.Where(c => c.Email.Contains(filter.Keyword)
                || c.Email.Contains(filter.Keyword)
                || c.Phone.Contains(filter.Keyword));
            var count = query.Count();
            var listItems = query.OrderByDescending(o => o.Id).Skip(filter.PageSize * (filter.Page - 1)).Take(filter.PageSize).ToList();

            var modelItems = _mapper.Map<List<UserModel>>(listItems);
            var model = new DataList<UserModel>(modelItems, filter.Page, filter.PageSize, count);
            return model;
        }

        public DataList<UserModel> FindShopUsers(ParamFilter filter)
        {
            filter.Include += "";
            filter.Exclude += "";
            var query = _dbContext.User.Where(c => c.Email.Contains(filter.Keyword)
                || c.Email.Contains(filter.Keyword)
                || c.Phone.Contains(filter.Keyword));
            query = query.Where(c => c.UserType != (int)UserTypes.Admin);
            var include = filter.Include.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (include.Length > 0)
            {
                query = _dbContext.User.Where(c => include.Contains(c.Id + ""));
            }
            var exclude = filter.Exclude.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (exclude.Length > 0)
            {
                query = query.Where(c => !exclude.Contains(c.Id + ""));
            }


            var count = query.Count();
            var listItems = query.OrderByDescending(o => o.Id).Skip(filter.PageSize * (filter.Page - 1)).Take(filter.PageSize).ToList();

            var modelItems = _mapper.Map<List<UserModel>>(listItems);
            var model = new DataList<UserModel>(modelItems, filter.Page, filter.PageSize, count);
            return model;
        }
        public UserModel GetUserModel(int id)
        {
            var entity = _dbContext.User.FirstOrDefault(c => c.Id == id);
            var model = _mapper.Map<UserModel>(entity);
            if (model == null) model = new UserModel { 
                Password = Constants.DefaultPassword
            };
            if(model.Birthday != null)
            {
                model.BirthdayText = model.Birthday.Value.ToString("dd/MM/yyyy");
            }
            if (string.IsNullOrEmpty(model.Image)) model.Image = Constants.DefaultAvatar;
            return model;
        }
        public Respondent SaveUser(UserModel model)
        {
            model.Email = model.Email.ToLower().Trim();
            var currentTime = DateTime.Now;
            var response = new Respondent();
            var currentUserId = _httpContextAccessor.HttpContext.GetId();
            var currentUser = _dbContext.User.FirstOrDefault(c => c.Id == currentUserId);
            if(currentUser == null)
            {
                response.Name = nameof(model.Email);
                response.Message = Messages.ActionInvalid;
                return response;
            }
            var checkExisted = _dbContext.User.FirstOrDefault(c => c.Id != model.Id && c.Email == model.Email); 
            if (checkExisted != null)
            {
                response.Name = nameof(model.Email);
                response.Message = string.Format(Messages.DataExisted, nameof(model.Email));
                return response;
            }
            var entity = _dbContext.User.FirstOrDefault(c => c.Id == model.Id);
            if (entity == null)
            {
                entity = new User
                {
                    InsertTime = currentTime,
                    Email = model.Email
                };
                _dbContext.User.Add(entity);
            };
            entity.FullName = model.FullName;
            entity.Address = model.Address;
            entity.Phone = model.Phone;
            entity.Status = model.Status;
            entity.TestingMode = model.TestingMode;

            if (model.BirthdayText != null || model.Birthday != null)
            {
                entity.Birthday = TextUtilities.ConvertToDatetime(model.BirthdayText);
            }
            if(entity.Id != currentUserId && currentUser.UserType == (int)UserTypes.Admin)
            {
                entity.UserType = model.UserType;
            }
            entity.Notes = model.Notes;
            if(model.ImageFile != null)
            {
                entity.Image = FormFileUtilities.SaveImage(model.ImageFile, Constants.UploadFolder);
            }
            entity.UpdateTime = currentTime;
            _dbContext.SaveChanges();
            return new Respondent
            {
                Success = true,
                Message = string.Format(Messages.SaveSuccess, "User")
            };
        }

        public static SettingModel GetSettingModel(WashMachineContext dbContext)
        {
            var settingType = typeof(SettingModel);
            var settings = new SettingModel();
            var listSettings = dbContext.Setting.ToList();
            foreach (var item in listSettings)
            {
                if (item == null) continue;
                if (item.Key == null) continue;
                var piInstance = settingType.GetProperty(item.Key);
                if (piInstance == null) continue;
                piInstance.SetValue(settings, item.Value);
            }
            return settings;
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
            systemInfo.User.Rights = new List<string> { EnumUtilities.GetName<UserTypes>(systemInfo.User.UserType) };
            //if (systemInfo.User.Missions == null) systemInfo.User.Missions = new List<string>();


            // 2.Get system setting
            systemInfo.Setting = GetSettingModel(_dbContext);

            // 3.Get list shops
            var listShopIds = _dbContext.ShopOwner.Where(c => c.OwnerId == systemInfo.User.Id && c.Active != false).Select(s => s.ShopId).ToList();
            var shops = _dbContext.Shop.Where(c => systemInfo.User.UserType == (int)UserTypes.Admin || listShopIds.Contains(c.Id)).ToList();
            var listShops = shops.Select(s => new SelectItem { Id = s.Id, Code = s.Code, Name = s.Name }).ToList();
            systemInfo.ListShops = listShops;

            var shopCode = _httpContextAccessor.HttpContext.Request.Cookies[Business.Constants.ShopCodeCookie];
            systemInfo.User.ShopCode = shopCode;
            var shop = _dbContext.Shop.FirstOrDefault(c => c.Code == shopCode);
            systemInfo.ClientShop = _mapper.Map<ShopModel>(shop);
            return systemInfo;
        }

        public Respondent ChangePassword(ChangePasswordModel model)
        {
            var response = new Respondent();

            // 1.Check user exists
            var entity = _dbContext.User.FirstOrDefault(c => c.Email == model.Email);
            if(entity == null)
            {
                response.Name = nameof(model.Email);
                response.Message = Messages.ItemNotExisted;
                return response;
            }

            // 2.Check password of the user
            var encodePassword = TextUtilities.Encryption(model.CurrentPassword, entity.SaltKey);
            if (encodePassword != entity.Password)
            {
                response.Name = nameof(model.CurrentPassword);
                response.Message = Messages.IncorectPassword;
                return response;
            }

            var currentTime = DateTime.Now;
            entity.Password = encodePassword;

            entity.UpdateTime = currentTime;
            _dbContext.SaveChanges();

            response.Success = true;
            response.Message = Messages.SaveSuccess;
            return response;
        }
        public Respondent ResetPassword(int id)
        {
            var response = new Respondent();
            var userId = _httpContextAccessor.HttpContext.GetId();
            if (id == userId)
            {
                response.Message = Messages.ActionInvalid;
                return response;
            }
            var entity = _dbContext.User.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                response.Message = Messages.ItemNotExisted;
                return response;
            }
            var encodePassword = TextUtilities.Encryption(Constants.DefaultPassword, entity.SaltKey);
            entity.Password = encodePassword;
            
            _dbContext.SaveChanges();

            response.Success = true;
            response.Message = Messages.ResetPasswordSuccess;
            return response;
        }
        public Respondent DeleteUser(int id)
        {
            var response = new Respondent();
            var userId = _httpContextAccessor.HttpContext.GetId();
            if(id == userId)
            {
                response.Message = Messages.ActionInvalid;
                return response;
            }
            var entity = _dbContext.User.FirstOrDefault(c => c.Id == id);
            if(entity == null)
            {
                response.Message = Messages.ItemNotExisted;
                return response;
            }
            var listItems = _dbContext.ShopOwner.Where(c => c.OwnerId == id).ToList();
            _dbContext.ShopOwner.RemoveRange(listItems);
            _dbContext.User.Remove(entity);
            _dbContext.SaveChanges();

            response.Success = true;
            response.Message = Messages.DeletedSuccess;
            return response;
        }

        public List<UserModel> GetUsers()
        {
            return _mapper.Map<List<UserModel>>(_dbContext.User.ToList());
        }
    }
}
