using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public interface IShopService
    {
        Task<PaymentModel> CreateNewPayment(PaymentModel paymentModel);
        Task<OrderModel> CompletePayment(OrderModel orderModel);
        Task<PaymentModel> CancelPayment(PaymentModel paymentModel);
        Task<PaymentModel> UpdatePayment(PaymentModel paymentModel);
    }
}
