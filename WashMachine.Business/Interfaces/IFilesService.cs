using WashMachine.Business.Models;
using Libraries;
using System.IO;

namespace WashMachine.Business.Interfaces
{
    public interface IFilesService
    {
        Respondent SaveFile(FileModel model);
        Respondent GetDownloadFilesInfo(string time);
        FileInfo DownloadFile(string filePath);
    }
}
