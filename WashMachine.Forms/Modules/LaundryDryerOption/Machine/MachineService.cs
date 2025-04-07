using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using WashMachine.Forms.Services.Machine;

namespace WashMachine.Forms.Modules.LaundryDryerOption.Machine
{
    public class MachineService : IMachineService
    {
        SerialPort _serialPort;

        public MachineService()
        {

        }

        public Task<bool> ConnectAsync(string portName, int baudRate, int data, Parity parity = Parity.None, StopBits step = StopBits.One)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentNullException(nameof(portName));
            }

            try
            {
                Logger.Log($"CONNECT BEGIN: portName {portName} baudRate {baudRate} parity {parity} StopBits {step}");
                if (_serialPort == null)
                {
                    _serialPort = MachineManager.TryGetMachine(portName, baudRate, data, parity, step);
                    if (_serialPort != null && _serialPort.IsOpen == false)
                    {
                        _serialPort.Open();
                        Logger.Log($"CONNECT END: SUCCESSFULLY");
                        return Task.FromResult(true);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Log($"CONNECT END: FAILED");
                Disconect();
                if (Program.AppConfig.ForceAdminResetMachine == 1)
                {
                    MachineManager.Reset(portName);
                    _serialPort = MachineManager.TryGetMachine(portName, baudRate, data, parity, step);
                    if (_serialPort != null && _serialPort.IsOpen == false)
                    {
                        _serialPort.Open();
                        return Task.FromResult(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"CONNECT END: FAILED");
                Logger.Log(ex);
            }

            return Task.FromResult(false);
        }

        public void Disconect()
        {
            MachineManager.Disconnect(Program.AppConfig.DryerMachineCom, Program.AppConfig.DryerMachineBaudRate);
        }

        public void ExecCommand(string command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    throw new ArgumentNullException(nameof(command));
                }

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    Logger.Log("USING ExecCommand: " + command);
                    _serialPort.Write(command);
                    Logger.Log("_serialPort.Write ExecCommand: DONE");
                }
                else
                {
                    throw new Exception($"_serialPort is null or closed");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void ExecHexCommand(string hexCommand)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexCommand))
                {
                    throw new ArgumentNullException(nameof(hexCommand));
                }

                if (Program.AppConfig.HexTrimMethod == 1)
                {
                    hexCommand = hexCommand.Replace(" ", string.Empty);
                }

                if (_serialPort != null && _serialPort.IsOpen || true)
                {
                    byte[] buffer = HexStringToByteArray(hexCommand);
                    Logger.Log("USING ExecHexCommand: " + hexCommand);
                    _serialPort.Write(buffer, 0, buffer.Length);
                    Logger.Log("_serialPort.Write ExecHexCommand: DONE");
                }
                else
                {
                    throw new Exception($"_serialPort is null or closed");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public string HexByteArrayToString(byte[] bytes)
        {
            return string.Empty;
        }

        public List<string> CreateNewHexCommandByCounter(int counter, List<string> hexCommandsDefault)
        {
            if (hexCommandsDefault == null)
            {
                throw new ArgumentNullException(nameof(hexCommandsDefault));
            }

            if (hexCommandsDefault.Count == 0)
            {
                throw new Exception($"{nameof(hexCommandsDefault)} can not be empty.");
            }

            if (counter == 0)
            {
                throw new Exception($"{nameof(counter)} can not be Zero.");
            }

            int startIndex = 9;

            List<string> hexList = hexCommandsDefault;
            string xorHexValue = hexList[startIndex].ToString();
            int value = Convert.ToInt32(xorHexValue, 16);

            if (value < int.MaxValue)
            {
                value += counter;
                string hexValue = value.ToString("X4");
                Console.WriteLine($"HEX VALUE: {hexValue}");
                int step = 1;
                string reverseHexData = string.Empty;
                int hexValueLength = hexValue.Length;

                while (hexValueLength > 0)
                {
                    if (step <= 2)
                    {
                        char charHex = hexValue[hexValueLength - 1];
                        reverseHexData += charHex;
                        step++;
                        hexValueLength -= 1;
                    }

                    if (step > 2)
                    {
                        IEnumerable<char> reverseHexs = reverseHexData.Reverse();
                        hexList[startIndex] = $"0x" + string.Concat(reverseHexs);
                        startIndex -= 1;
                        step = 1;
                        reverseHexData = string.Empty;
                    }
                }

                return hexList;
            }
            else
            {
                throw new Exception($"The counter value {counter} exceed int.MaxValue");
            }
        }

        public List<int> XorHexCommand(List<string> hexCommands)
        {
            if (hexCommands == null)
            {
                throw new ArgumentNullException(nameof(hexCommands));
            }

            if (hexCommands.Count == 0)
            {
                throw new Exception($"{nameof(hexCommands)} can not be empty.");
            }

            List<int> xorHexCommands = new List<int>();
            string xorHexCommandData = string.Empty;

            for (int i = 0; i < hexCommands.Count;)
            {
                if (xorHexCommands.Count == 0)
                {
                    Console.WriteLine($"XorHexCommand CASE 1::: {hexCommands[i]} {hexCommands[i + 1]}");
                    int firstHex = Convert.ToInt32(hexCommands[i], 16);
                    int lastHex = Convert.ToInt32(hexCommands[i + 1], 16);
                    int resultXorHex = firstHex ^ lastHex;
                    xorHexCommands.Add(resultXorHex);
                    xorHexCommandData += resultXorHex.ToString("X2") + " ";
                    i += 2;
                }
                else
                {
                    Console.WriteLine($"XorHexCommand CASE 2::: {xorHexCommands[i - 2]} {hexCommands[i]}");

                    int firstHex = xorHexCommands[i - 2];
                    int lastHex = Convert.ToInt32(hexCommands[i], 16);
                    int resultXorHex = firstHex ^ lastHex;
                    xorHexCommands.Add(resultXorHex);
                    xorHexCommandData += resultXorHex.ToString("X2") + " ";
                    i++;
                }

                Console.WriteLine($"XorHexCommand RESULT: {xorHexCommandData}");
            }

            return xorHexCommands;
        }

        public string[] ConvertNumberHexToCommand(int number)
        {
            string hexValue = number.ToString("X4");

            string first = hexValue.Substring(0, 2);
            string last = hexValue.Substring(2, 2);

            return new string[] { last, first };
        }
    }
}
