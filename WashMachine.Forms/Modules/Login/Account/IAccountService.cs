using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.Login.Account
{
    public interface IAccountService
    {
        Task<string> SignIn(string username, string password);
    }
}
