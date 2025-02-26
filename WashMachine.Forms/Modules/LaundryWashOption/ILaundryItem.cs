using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryWashOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        string ImplementCommand { get; }
        Dictionary<string, string> ProgramCommands { get; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
        Task Start();
    }
}
