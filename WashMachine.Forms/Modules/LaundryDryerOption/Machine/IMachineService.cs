using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.LaundryDryerOption.Machine
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

        /// <summary>
        /// Input "01 03 14 00 02 00 01 00 01 00 01 00 00 00 00 00 00 00 00 00 00 00 00 48 CE"
        /// Expected "01 03 14 00 02 00 01 00 01 00 01 00 00 00 00 00 00 00 00 00 00 00 00 48 CE" 
        /// Using Cyclic Redundancy Check (CRC) to validate last two bytes [48 CE]
        /// </summary>
        /// <param name="hexCommand"></param>
        /// <returns></returns>
        bool ValidateCRCCommand(string hexCommand);

        /// <summary>
        /// Input 01 06 01 68 00 01 C8 2A
        /// Expected 01 06 01 68 00 01 C8 2A
        /// </summary>
        /// <param name="hexSended"></param>
        /// <param name="hexRecived"></param>
        /// <returns></returns>
        bool ValidateCommand(string hexSended, string hexRecived);
    }
}
