using AutoMapper;
using BotDetect;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WashMachine.Web.ApiControllers
{
    public class OctopusApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public OctopusApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        public Respondent GetConfig()
        {
            try
            {
                SettingService service = _business.GetService<SettingService>();
                OctopusSettingModel settingModel = service.GetOctopusSetting();
                return new Respondent() { Success = true, DataId = 1, Data = settingModel };

            }
            catch (Exception e)
            {
                return new Respondent() { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        public Respondent UploadFiles(FileModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                return new Respondent
                {
                    Message = "Receive upload from client with no file found."
                };
            }

            try
            {
                IFilesService filesService = _business.GetService<FilesService>();
                var result = filesService.SaveFile(model);
                return result;
            }
            catch (Exception ex)
            {
                return new Respondent
                {
                    Message = ex.Message
                };
            }
        }

        public Respondent GetDownloadFilesInfo(string time)
        {
            IFilesService filesService = _business.GetService<FilesService>();
            return filesService.GetDownloadFilesInfo(time);
        }

        public IActionResult DownloadFile(string filePath)
        {
            SettingService service = _business.GetService<SettingService>();
            OctopusSettingModel settingModel = service.GetOctopusSetting();

            filePath = Path.Combine(settingModel.OctopusDownloadFolder, filePath);

            var connection = HttpContext.Request.Headers["Connection"];
            var keepAlive = !string.IsNullOrEmpty(connection) && connection.Contains("Keep-Alive");
            IFilesService filesService = _business.GetService<FilesService>();
            var result = filesService.DownloadFile(filePath);
            if (result == null) return Content("File not found");

            var encryptFile = CryptLibrary.EncryptFile(result.FullName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(encryptFile);
            CryptLibrary.DeleteFile(encryptFile);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Xml, encryptFile);
        }
    }
}
