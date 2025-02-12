using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Libraries
{
    public class FormFileUtilities
    {
        public static string SaveImage(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0) return string.Empty;
            var path = string.Format("{0}/wwwroot/{1}", Directory.GetCurrentDirectory(), folderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = TextUtilities.ConvertToSlug(file.FileName);
            string uploadsForder = Path.Combine(path);
            string filePath = Path.Combine(uploadsForder, fileName);

            var dotIndex = file.FileName.LastIndexOf(".");
            if (dotIndex < 0) dotIndex = file.FileName.Length - 1;
            var firtName = file.FileName.Substring(0, dotIndex);
            var extension = file.FileName.Substring(dotIndex);
            var i = 0;
            while (File.Exists(filePath))
            {
                i++;
                var tempName = $"{firtName}-{i}{extension}";
                fileName = TextUtilities.ConvertToSlug(tempName);
                filePath = Path.Combine(uploadsForder, fileName);
            }

            using (var stream = File.Create(filePath))
            {
                file.CopyTo(stream);
            }
            fileName = string.Format("/{0}/{1}", folderPath, fileName);
            return fileName;
        }
    }
    
}
