using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Mvc;

namespace WashMachine.Web.ApiControllers
{
    public class WashMachineApiController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public WashMachineApiController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        [HttpPost("payme-notification-test")]
        public Respondent PaymeNotification([FromBody] Respondent notification)
        {
            var response = new Respondent();
            response.Message = "This is the messsage";
            return response;
        }

        [HttpPost]
        public Respondent UploadFiles(FileModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                var message = "Receive upload from client with no file found";
                FileUtilities.LogFile(message);
                return new Respondent
                {
                    Message = message
                };
            }

            FileUtilities.LogFile("Receive files from client: " + model.Files.Count);
            var service = _business.GetService<FilesService>();
            try
            {
                var result = service.SaveFile(model);
                return result;
            }
            catch (Exception ex)
            {
                FileUtilities.LogFile(ex);
                return new Respondent
                {
                    Message = ex.Message
                };
            }
        }

        public Respondent DownloadFilesInfo(string time)
        {
            //TestOnlineKey();
            var service = _business.GetService<FilesService>();
            return service.GetDownloadFilesInfo(time);
        }

        public IActionResult DownloadFile(string file)
        {
            var filePath = file;
            var connection = HttpContext.Request.Headers["Connection"];
            var keepAlive = !string.IsNullOrEmpty(connection) && connection.Contains("Keep-Alive");

            var service = _business.GetService<FilesService>();
            var result = service.DownloadFile(filePath);
            if (result == null) return Content("File not found");

            var encryptFile = CryptLibrary.EncryptFile(result.FullName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(encryptFile);
            CryptLibrary.DeleteFile(encryptFile);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Xml, encryptFile);
        }

        public IActionResult PosSetting()
        {
            var systemInfo = HttpContext.GetSystemInfo();
            var xFileTime = systemInfo.Setting.XFileHour + systemInfo.Setting.XFileMinute;
            var uploadTime = systemInfo.Setting.UploadHour + systemInfo.Setting.UploadMinute;
            var downloadTime = systemInfo.Setting.DownloadHour + systemInfo.Setting.DownloadMinute;

            var code = $"{xFileTime}_{uploadTime}_{downloadTime}";
            return Content(code);
        }

        private void TestOnlineKey()
        {
            var filePath = "Dowload02.txt";
            var service = _business.GetService<FilesService>();
            var result = service.DownloadFile(filePath);

            const string PublicKeyFile = @"SecurityKeys\Test\rsaPublicKey.txt";
            const string PrivateKeyFile = @"SecurityKeys\Test\rsaPrivateKey.txt";

            var publicKey = CryptLibrary.ReadFile(PublicKeyFile);
            var privateKey = CryptLibrary.ReadFile(PrivateKeyFile);

            var encryptFile = CryptLibrary.EncryptFile(result.FullName);

            var fileName = CryptLibrary.DecryptFile(encryptFile);
        }
    }
}
