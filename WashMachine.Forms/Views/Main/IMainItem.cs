using System.Windows.Forms;

namespace WashMachine.Forms.Views.Main
{
    public interface IMainItem
    {
        void Click();
        Control GetTemplate();
    }
}
