using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WashMachine.Forms.Services.Machine;

namespace WashMachine.Forms.Modules.LaundryWashOption.Machine
{
    public class MachineService : IMachineService
    {
        SerialPort _serialPort;
        public event EventHandler<EventArgs> DataReceived;

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
                    if (_serialPort != null)
                    {
                        _serialPort.DataReceived += SerialPort_DataReceived;
                        if (_serialPort.IsOpen == false)
                        {
                            _serialPort.Open();
                            Logger.Log($"CONNECT END: SUCCESSFULLY");
                        }
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
                    Logger.Log($"UnauthorizedAccessException RECONNECT END: FAILED");
                    MachineManager.Reset(portName);
                    _serialPort = MachineManager.TryGetMachine(portName, baudRate, data, parity, step);
                    if (_serialPort != null)
                    {
                        _serialPort.DataReceived += SerialPort_DataReceived;
                        if (_serialPort.IsOpen == false)
                        {
                            _serialPort.Open();
                            Logger.Log($"CONNECT END: SUCCESSFULLY");
                        }
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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            DataReceived?.Invoke(data, e);
            Console.WriteLine($"Data Received: {data}");
        }

        public void Disconect()
        {
            MachineManager.Disconnect(Program.AppConfig.WashMachineCom, Program.AppConfig.WashMachineBaudRate);
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

        public void RemoveRegisterEvents()
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
            }
        }

        public bool ValidateCRCCommand(string hexCommand)
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

                byte[] buffer = HexStringToByteArray(hexCommand);

                // Modbus RTU frame (excluding CRC bytes at the end)
                // Remove two last buffer
                byte[] frame = buffer.Skip(0).Take(buffer.Length - 2).ToArray();

                ushort computedCRC = CalculateCRC(frame);
                Console.WriteLine($"Computed CRC: {computedCRC:X4}");
                // Provided CRC in little-endian format
                byte[] providedCRC = { buffer.ElementAt(buffer.Length - 2), buffer.Last() }; // Interpreted as CE48

                // Compare computed CRC to the provided CRC
                ushort providedCRCValue = BitConverter.ToUInt16(providedCRC, 0); // Convert little-endian bytes to ushort
                Console.WriteLine($"Computed CRC: {computedCRC:X4}");
                Console.WriteLine($"Provided CRC: {providedCRCValue:X4}");
                Console.WriteLine(computedCRC == providedCRCValue ? "CRC matches. Frame is valid!" : "CRC mismatch. Frame is invalid!");

                return computedCRC == providedCRCValue;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        private ushort CalculateCRC(byte[] data)
        {
            ushort crc = 0xFFFF; // Start with all bits set
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i]; // XOR byte into least significant byte of CRC

                for (int j = 0; j < 8; j++) // Process each bit
                {
                    if ((crc & 0x0001) != 0) // If the least significant bit is set
                    {
                        crc >>= 1;           // Shift right
                        crc ^= 0xA001;       // XOR with polynomial 0xA001
                    }
                    else
                    {
                        crc >>= 1;           // Just shift right
                    }
                }
            }

            return crc; // Final CRC value
        }

        public void FakeInvokeDataReceived(string data = null)
        {
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                if (!string.IsNullOrWhiteSpace(data))
                {
                    DataReceived?.Invoke(data, null);
                }
                else
                {
                    DataReceived?.Invoke("01 03 14 00 02 00 01 00 01 00 01 00 00 00 00 00 00 00 00 00 00 00 00 48 CE", null);
                }
            });
        }
    }
}
