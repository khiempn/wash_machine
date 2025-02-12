using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public interface IPaidByService
    {
        Task StartScanning();
        void StopScan();
        Task StartScanning(Form mainForm);
        void StopScan(Form mainForm);
        Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter);
    }
}
