using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryDryerOption
{
    public interface ILaundryOptionItem
    {
        string Name { get; }
        /// <summary>
        /// Temperature Command List
        /// </summary>
        Dictionary<string, string> TemperatureCommands { get; set; }
        /// <summary>
        /// Time Command List
        /// </summary>
        Dictionary<string, string> TimeCommands { get; set; }
        /// <summary>
        /// Start Command
        /// </summary>
        string ImplementCommand { get; set; }
        string StopCommand { get; set; }
        string HealthCheckCommand { get; set; }
        Action<object> HealthCheckCompleted { get; set; }
        void Click();
        Control GetTemplate();
        void DisableItem(Control control);
        Task Start(OrderModel order);
        void SetIsRunning();
        Task HealthCheck();
    }
}
