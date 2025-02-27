using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.LaundryDryerOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        /// <summary>
        /// Temperature Command List
        /// </summary>
        Dictionary<string, string> TemperatureCommands { get; }
        /// <summary>
        /// Time Command List
        /// </summary>
        Dictionary<string, string> TimeCommands { get; }
        /// <summary>
        /// Start Command
        /// </summary>
        string ImplementCommand { get; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
        Task Start();
        void SetIsRunning();
    }
}
