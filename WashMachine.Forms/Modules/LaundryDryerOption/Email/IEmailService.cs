using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryDryerOption.Email
{
    public interface IEmailService
    {
        void SendEmailStartError(OrderModel order, string machineName, string commandError);

        void SendEmailHealthCheckError(string machineName, string commandError);
    }
}
