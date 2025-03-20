using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode.Attributes;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize(Mission = Roles.Admin)]
    public class SettingController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public SettingController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        public IActionResult Index()
        {
            return GeneralSetting();
        }

        public IActionResult GeneralSetting()
        {
            var service = _business.GetService<SettingService>();
            var userInfo = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(userInfo.Id);
            var systemInfo = service.GetSystemInfo(userId);
            return View("GeneralSetting", systemInfo.Setting);
        }

        [HttpPost]
        public IActionResult GeneralSetting(SettingModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var service = _business.GetService<SettingService>();
            var result = service.SaveSetting(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);
            }
            TempData.SetMessage(result.Message);

            var userInfo = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(userInfo.Id);
            var systemInfo = service.GetSystemInfo(userId);
            return View("GeneralSetting", systemInfo.Setting);
        }

        public IActionResult OctopusSetting()
        {
            var service = _business.GetService<SettingService>();
            var userInfo = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(userInfo.Id);
            var systemInfo = service.GetSystemInfo(userId);
            return View(systemInfo.Setting);
        }

        [HttpPost]
        public IActionResult OctopusSetting(SettingModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var service = _business.GetService<SettingService>();
            var result = service.SaveOctopusSetting(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);
            }
            TempData.SetMessage(result.Message);

            var userInfo = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(userInfo.Id);
            var systemInfo = service.GetSystemInfo(userId);
            return View("OctopusSetting", systemInfo.Setting);
        }


        public IActionResult WashmachineCommand()
        {
            var service = _business.GetService<MachineCommadService>();
            var userInfo = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(userInfo.Id);
            var machineCommandModel = service.GetAll();
            return View("WashmachineCommand", machineCommandModel);
        }

        [HttpPost]
        public IActionResult WashmachineCommand(MachineCommandModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var IsResetDefault = HttpContext.Request.Form["IsResetDefault"].ToString().Equals("1");
            if (IsResetDefault)
            {
                model = new MachineCommandModel();
            }

            var service = _business.GetService<MachineCommadService>();
            var result = service.SaveAll(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);
            }
            TempData.SetMessage(result.Message);
            var machineCommandModel = service.GetAll();
            return View("WashmachineCommand", machineCommandModel);
        }
    }
}
