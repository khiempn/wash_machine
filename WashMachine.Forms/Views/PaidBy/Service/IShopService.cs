using WashMachine.Forms.Views.PaidBy.Service.Model;
using WashMachine.Forms.Views.PaidBy.Service.Octopus;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.PaidBy.Service
{
    public interface IShopService
    {
        Task<PaymentModel> CreateNewPayment(PaymentModel paymentModel);
        Task<OrderModel> CompletePayment(OrderModel orderModel);
        Task<PaymentModel> CancelPayment(PaymentModel paymentModel);
        Task<PaymentModel> UpdatePayment(PaymentModel paymentModel);
    }
}
