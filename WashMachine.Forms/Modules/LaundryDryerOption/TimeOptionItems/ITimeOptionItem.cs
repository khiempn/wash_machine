using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TimeOptionItems
{
    public interface ITimeOptionItem
    {
        void Click();
        Control GetTemplate();
    }
}
