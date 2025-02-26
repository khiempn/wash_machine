using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TimeOptionItems
{
    public interface ITimeOptionItem
    {
        string Name { get; }
        int TimeNumber { get; }
        void Click();
        Control GetTemplate();
    }
}
