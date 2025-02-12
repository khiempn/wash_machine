using WashMachine.Forms.Views.PaidBy.Service.Model;
using WashMachine.Forms.Views.PaidBy.Service.Octopus;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.PaidBy
{
    public interface IPaidByOctopusItem
    {
        Task StartScanning(PaymentModel payment);
        void StopScan();
        void Printer(OrderModel orderModel);
    }
}
