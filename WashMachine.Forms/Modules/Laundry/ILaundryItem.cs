using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Laundry
{
    public interface ILaundryItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
    }
}
