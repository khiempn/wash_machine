using WashMachine.Forms.Common.Http;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.Login.Account
{
    public class AccountService : IAccountService
    {
        HttpService httpService;
        public AccountService()
        {
            httpService = new HttpService();
        }

        public async Task<string> SignIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Please enter username.";
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Please enter password.";
            }

            string result = await httpService.Post($"{Program.AppConfig.AppHost}/UserApi/SignIn?username={username}&password={password}", new { });

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);

            if (response == null || response.Success == false)
            {
                return "You have no permission to access.";
            }
            else
            {
                Logger.Log(result);
            }
            return string.Empty;
        }
    }
}
