using WashMachine.Forms.Modules.PaidBy.PaidByItems.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy
{
    public interface IPaidByItem
    {
        Control GetTemplate();
        void Click();
        Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter);
        Task TrackingStatistics(StatisticsModel statistics);
    }
}
