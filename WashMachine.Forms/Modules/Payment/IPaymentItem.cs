using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Payment
{
    public interface IPaymentItem
    {
        int CoinExchange { get; }
        float Amount { get; }
        int PaymentAmount { get; }
        List<string> HexCommandsDefault { get; set; }
        List<string> HexCommandsExpected { get; set; }

        Task PaymentAsync();
        Task DropCoinAsync(int orderId);
        Control GetTemplate();
        Task ClickAsync();
    }
}
