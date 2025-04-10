using System;
using System.Windows.Forms;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.Payment
{
    public interface IPaymentItem
    {
        int PaymentAmount { get; set; }
        void SetAmount(int paymentAmount);
        int GetAmount();
        Action<Form, Action, OrderModel> OnPaymentCompleted { get; set; }
    }
}
