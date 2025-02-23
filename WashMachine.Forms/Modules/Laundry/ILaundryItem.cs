using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Laundry
{
    public interface ILaundryItem
    {
        void Click();
        Control GetTemplate();
    }
}
