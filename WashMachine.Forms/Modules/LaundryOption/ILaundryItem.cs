using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
    }
}
