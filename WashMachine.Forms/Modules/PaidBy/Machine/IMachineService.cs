using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Machine
{
    public interface IMachineService
    {
        /// <summary>
        /// Create new SerialPort
        /// </summary>
        /// <param name="portName">COM3</param>
        /// <param name="baudRate">9600</param>
        /// <param name="data">None</param>
        /// <param name="parity">8</param>
        /// <param name="step">One</param>
        /// <returns></returns>
        Task<bool> ConnectAsync(string portName, int baudRate, int data, Parity parity = Parity.None, StopBits step = StopBits.One);
        /// <summary>
        /// Remove serialPort connect
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Create new event main form keypress
        /// </summary>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        Task<bool> ConnectAsync(Form mainForm);
        /// <summary>
        /// Remove event main form keypress
        /// </summary>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        Task<bool> Disconnect(Form mainForm);

        OctopusService ConnectOctopusAsync();
        Task<bool> DisconnectTimer();
    }
}
