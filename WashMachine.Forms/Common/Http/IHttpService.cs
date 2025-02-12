using System.Threading.Tasks;

namespace WashMachine.Forms.Common.Http
{
    public interface IHttpService
    {
        Task<string> Post(string url, object data);
        Task<string> Get(string url);
        T ConvertTo<T>(string data);
    }
}
