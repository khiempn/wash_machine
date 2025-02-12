using WashMachine.Forms.Modules.PaidBy.PaidByItems.Model;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Service.Eft
{
    public interface IEftPayService
    {
        Task<EftPayResponseModel> Sale(EftPayRequestModel eftPayParameter);
    }
}
