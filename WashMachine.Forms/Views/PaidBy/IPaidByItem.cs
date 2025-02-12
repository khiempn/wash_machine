using WashMachine.Forms.Views.PaidBy.PaidByItems.Model;
using WashMachine.Forms.Views.PaidBy.Service.Eft;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.PaidBy
{
    public interface IPaidByItem
    {
        Control GetTemplate();
        void Click();
        Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter);
        Task TrackingStatistics(StatisticsModel statistics);
    }
}
