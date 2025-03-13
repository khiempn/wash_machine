using WashMachine.Forms.Modules.PaidBy.Service.Model;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public interface IOctopusService
    {
        void ShowOutOfService();

        bool Initial();

        bool DisconnectTimer();

        /// <summary>
        /// Start a timer to upload files or download file from server
        /// </summary>
        void RunAJobToCompleteFiles();

        Task RefreshConfig();

        OctopusService StartWaitingPayment(PaymentModel payment);

        void HideOutOfService();

        void SetCurrentForm(Form form);

        bool IsUserUsingApplication();
    }
}
