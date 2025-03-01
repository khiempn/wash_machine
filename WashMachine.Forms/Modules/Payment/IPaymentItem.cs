using System;

namespace WashMachine.Forms.Modules.Payment
{
    public interface IPaymentItem
    {
        int PaymentAmount { get; set; }
        void SetAmount(int paymentAmount);
        int GetAmount();
        Action<object> PaymentCompletedCallBack { get; set; }
    }
}
