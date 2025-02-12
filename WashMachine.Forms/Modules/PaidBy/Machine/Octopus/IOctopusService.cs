using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public interface IOctopusService
    {
        void FileLocked();

        bool Initial();

        bool Discope();

        /// <summary>
        /// Start a timer to upload files or download file from server
        /// </summary>
        void RunAJobToCompleteFiles();

        Task RefreshConfig();

        OctopusService StartWaitingPayment(PaymentModel payment);
    }
}
