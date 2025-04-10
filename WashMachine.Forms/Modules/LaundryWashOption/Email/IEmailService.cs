using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryWashOption.Email
{
    public interface IEmailService
    {
        void SendEmailStartError(OrderModel order, string machineName, string commandError);

        void SendEmailHealthCheckError(string machineName, string commandError);
    }
}
