using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
    }
}
