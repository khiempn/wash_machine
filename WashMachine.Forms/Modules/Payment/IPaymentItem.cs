using System;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Payment
{
    public interface IPaymentItem
    {
        int PaymentAmount { get; set; }
        void SetAmount(int paymentAmount);
        int GetAmount();
        Action<Form, Action> PaymentCompletedCallBack { get; set; }
    }
}
