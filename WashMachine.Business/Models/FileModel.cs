using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WashMachine.Business.Models
{
    public class FileModel
    {
        public string FolderName { get; set; }
        public List<IFormFile> Files { get; set; }
    }

    public class FileDownloadModel
    {
        public List<FileInfo> Files { get; set; }
    }
}
