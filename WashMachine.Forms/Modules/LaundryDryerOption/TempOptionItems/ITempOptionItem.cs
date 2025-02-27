using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems
{
    public interface ITempOptionItem
    {
        string Name { get; }
        int TypeId { get; }
        void Click();
        Control GetTemplate();
    }
}
