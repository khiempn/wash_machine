using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryWashOption.PaymentItems
{
    public interface IPaymentItem
    {
        void Click();
        Control GetTemplate();
    }
}
