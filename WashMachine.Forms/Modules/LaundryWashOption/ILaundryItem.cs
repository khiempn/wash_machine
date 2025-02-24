using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryWashOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
    }
}
