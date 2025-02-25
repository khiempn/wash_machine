using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems
{
    public interface ITimeOptionItem
    {
        string Name { get; }
        void Click();
        Control GetTemplate();
    }
}
