using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;
using Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WashMachine.Business.Services
{
    public class FilesService : IBusiness, IFilesService
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;
        public FilesService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Respondent SaveFile(FileModel model)
        {
            var response = new Respondent();
            if (model.Files == null || model.Files.Count == 0)
            {
                response.Message = "There is no file to upload";
                FileUtilities.LogFile(response.Message);
                return response;
            }

            var settings = AccessService.GetSettingModel(_dbContext);
            var folderPath = settings.OctopusUploadFolder;
            if (string.IsNullOrEmpty(folderPath)) folderPath = @"D:\Uploads";
            var path = folderPath + @"\" + model.FolderName;

            try
            {
                foreach (var file in model.Files)
                {
                    var fileName = SaveFile(file, path);
                }
            }
            catch (Exception ex)
            {
                FileUtilities.LogFile(ex);
                response.Message = ex.Message;
                return response;
            }

            response.Success = true;
            response.Message = $"Upload success {model.Files.Count} file";
            if (model.Files.Count > 1) response.Message += "s";
            FileUtilities.LogFile(response.Message);
            return response;
        }

        private string SaveFile(IFormFile file, string path)
        {
            if (file == null || file.Length == 0) return string.Empty;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = file.FileName;
            string uploadsForder = Path.Combine(path);
            string filePath = Path.Combine(uploadsForder, fileName);

            using (var stream = File.Create(filePath))
            {
                file.CopyTo(stream);
            }

            if (Regex.IsMatch(filePath, @"(\.enc)$"))
            {
                fileName = CryptLibrary.DecryptFile(filePath);
                CryptLibrary.DeleteFile(filePath);
            }

            fileName = $"/{fileName}";// string.Format("/{0}/{1}", folderName, fileName);
            return fileName;
        }

        public Respondent GetDownloadFilesInfo(string time)
        {
            //var currentMock = DateTime.Now.ToString("HHmm");
            var settings = AccessService.GetSettingModel(_dbContext);
            var folderPath = settings.OctopusDownloadFolder;
            if (string.IsNullOrEmpty(folderPath)) folderPath = @"D:\Downloads";

            var response = new Respondent();
            _dbContext.Log.Add(new Log() { Message = JsonConvert.SerializeObject(settings) });
           
            var listFiles = new List<string>();
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, "IBKL.*").ToList();
                _dbContext.Log.Add(new Log() { Message = files.Count.ToString() });

                var configTime = settings.DownloadHour + settings.DownloadMinute;
                if (configTime == time || true)
                {
                    files = Directory.GetFiles(folderPath, "OTP.*").ToList();

                    var blackList = Directory.GetFiles(folderPath, "BLKL.*").ToList();
                    files.AddRange(blackList);

                    var extended = Directory.GetFiles(folderPath, "FBLK.*");
                    files.AddRange(extended);

                    var functional = Directory.GetFiles(folderPath, "NBLK.*");
                    files.AddRange(functional);
                }
                foreach (var item in files)
                {
                    var fileName = Path.GetFileName(item);
                    listFiles.Add(fileName);
                }
            }
            _dbContext.Log.Add(new Log() { Message = JsonConvert.SerializeObject(response) });

            response.Data = listFiles;
            response.Success = true;

            _dbContext.SaveChanges();

            return response;
        }

        public FileInfo DownloadFile(string fileName)
        {
            _dbContext.Log.Add(new Log() { Message = fileName });

            if (File.Exists(fileName))
            {
                _dbContext.Log.Add(new Log() { Message = "ÁDASDAS" });

                var fileInfo = new FileInfo(fileName);
                return fileInfo;
            }
            _dbContext.SaveChanges();

            return null;
        }
    }
}
