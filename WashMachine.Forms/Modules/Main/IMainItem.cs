using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Main
{
    public interface IMainItem
    {
        void Click();
        Control GetTemplate();
    }
}
