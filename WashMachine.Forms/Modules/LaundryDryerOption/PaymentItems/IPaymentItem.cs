using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.PaymentItems
{
    public interface IPaymentItem
    {
        void Click();
        Control GetTemplate();
    }
}
