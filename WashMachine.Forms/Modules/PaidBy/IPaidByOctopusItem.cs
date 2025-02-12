using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy
{
    public interface IPaidByOctopusItem
    {
        Task StartScanning(PaymentModel payment);
        void StopScan();
        void Printer(OrderModel orderModel);
    }
}
