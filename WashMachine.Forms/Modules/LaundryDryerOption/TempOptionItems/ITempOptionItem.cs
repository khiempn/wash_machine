using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems
{
    public interface ITempOptionItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
    }
}
