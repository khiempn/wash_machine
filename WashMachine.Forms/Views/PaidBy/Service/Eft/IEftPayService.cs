using WashMachine.Forms.Views.PaidBy.PaidByItems.Model;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.PaidBy.Service.Eft
{
    public interface IEftPayService
    {
        Task<EftPayResponseModel> Sale(EftPayRequestModel eftPayParameter);
    }
}
