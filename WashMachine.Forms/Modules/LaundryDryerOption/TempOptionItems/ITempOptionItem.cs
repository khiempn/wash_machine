using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems
{
    public interface ITempOptionItem
    {
        void Click();
        Control GetTemplate();
    }
}
