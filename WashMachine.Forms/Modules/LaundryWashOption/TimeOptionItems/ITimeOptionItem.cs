using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems
{
    public interface ITimeOptionItem
    {
        void Click();
        Control GetTemplate();
    }
}
