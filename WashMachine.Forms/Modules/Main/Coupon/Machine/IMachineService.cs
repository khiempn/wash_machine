using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Main.Coupon.Machine
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
       
        void Disconect();
        
        /// <summary>
        /// AA OE 01 D1 02 00 00 00 00 00 01 27 10 14 00 FE DD
        /// </summary>
        /// <param name="hexCommand"></param>
        void ExecHexCommand(string hexCommand);

        void ExecCommand(string command);

        /// <summary>
        /// Using default HexCommand to generate new HexCommand 
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="hexCommandsDefault"></param>
        List<string> CreateNewHexCommandByCounter(int counter, List<string> hexCommandsDefault);

        /// <summary>
        /// Using xor to generate new command to drop coin
        /// </summary>
        /// <param name="hexCommands"></param>
        /// <returns></returns>
        List<int> XorHexCommand(List<string> hexCommands);

        Task<bool> ConnectAsync(Form mainForm);
        Task<bool> Disconnect(Form mainForm);
    }
}
