using System.Threading.Tasks;

namespace WashMachine.Forms.Views.Login.Account
{
    public interface IAccountService
    {
        Task<string> SignIn(string username, string password);
    }
}
