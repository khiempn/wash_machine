using System;
using System.Windows.Forms;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.Payment.PaymentItems
{
    public class HkdPaymentItem : IPaymentItem
    {
        public int PaymentAmount { get; set; }
        public Action<Form, Action, OrderModel> OnPaymentCompleted { get; set; }

        public HkdPaymentItem()
        {
            
        }

        public HkdPaymentItem(int paymentAmount)
        {
            PaymentAmount = paymentAmount;
        }

        public int GetAmount()
        {
            return PaymentAmount;
        }

        public void SetAmount(int paymentAmount)
        {
            PaymentAmount = paymentAmount;
        }
    }
}
